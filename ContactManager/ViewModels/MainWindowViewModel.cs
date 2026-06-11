using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ContactManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private Contact? _selectedContact;
    private string _searchText = string.Empty;
    
    private string _fullNameInput = string.Empty;
    private string _emailInput = string.Empty;
    private string _phoneInput = string.Empty;
    private Department? _selectedDepartmentInput;

    private int _currentWindowIndex = 0;

    public ObservableCollection<Contact> Contacts { get; set; } = new();
    public ObservableCollection<Department> Departments { get; set; } = new();

    public string SearchText
    {
        get => _searchText;
        set { _searchText = value; OnPropertyChanged(nameof(SearchText)); _ = LoadDataAsync(); }
    }

    public Contact? SelectedContact
    {
        get => _selectedContact;
        set
        {
            _selectedContact = value;
            OnPropertyChanged(nameof(SelectedContact));
            OnPropertyChanged(nameof(IsContactSelected));

            if (_selectedContact != null)
            {
                FullNameInput = _selectedContact.FullName;
                EmailInput = _selectedContact.Email;
                PhoneInput = _selectedContact.Phone;
                SelectedDepartmentInput = Departments.FirstOrDefault(d => d.Id == _selectedContact.DepartmentId);
            }
        }
    }

    public bool IsContactSelected => SelectedContact != null;

    public bool IsFormValid => !string.IsNullOrWhiteSpace(FullNameInput) && !string.IsNullOrWhiteSpace(EmailInput);
    public bool ShowFullNameError => string.IsNullOrWhiteSpace(FullNameInput);
    public bool ShowEmailError => string.IsNullOrWhiteSpace(EmailInput);

    public string FullNameInput
    {
        get => _fullNameInput;
        set 
        { 
            _fullNameInput = value; 
            OnPropertyChanged(nameof(FullNameInput)); 
            OnPropertyChanged(nameof(IsFormValid));
            OnPropertyChanged(nameof(ShowFullNameError));
        }
    }

    public string EmailInput
    {
        get => _emailInput;
        set 
        { 
            _emailInput = value; 
            OnPropertyChanged(nameof(EmailInput)); 
            OnPropertyChanged(nameof(IsFormValid));
            OnPropertyChanged(nameof(ShowEmailError));
        }
    }

    public string PhoneInput
    {
        get => _phoneInput;
        set { _phoneInput = value; OnPropertyChanged(nameof(PhoneInput)); }
    }

    public Department? SelectedDepartmentInput
    {
        get => _selectedDepartmentInput;
        set { _selectedDepartmentInput = value; OnPropertyChanged(nameof(SelectedDepartmentInput)); }
    }

    public int CurrentWindowIndex
    {
        get => _currentWindowIndex;
        set
        {
            _currentWindowIndex = value;
            OnPropertyChanged(nameof(CurrentWindowIndex));
            OnPropertyChanged(nameof(WindowStatusText));
            OnPropertyChanged(nameof(IsViewWindowVisible));
            OnPropertyChanged(nameof(IsAddWindowVisible));
            OnPropertyChanged(nameof(IsEditWindowVisible));
        }
    }

    public string WindowStatusText => CurrentWindowIndex switch
    {
        0 => "Поиск и просмотр",
        1 => "Добавление контакта",
        2 => "Редактирование контакта",
        _ => "Поиск и просмотр"
    };

    public bool IsViewWindowVisible => CurrentWindowIndex == 0;
    public bool IsAddWindowVisible => CurrentWindowIndex == 1;
    public bool IsEditWindowVisible => CurrentWindowIndex == 2;

    public MainWindowViewModel(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
        _ = LoadDataAsync();
    }

    public void MoveNext()
    {
        if (CurrentWindowIndex < 2) CurrentWindowIndex++;
        else CurrentWindowIndex = 0;
    }

    public void MovePrev()
    {
        if (CurrentWindowIndex > 0) CurrentWindowIndex--;
        else CurrentWindowIndex = 2;
    }

    public void GoToEditWindow()
    {
        if (SelectedContact != null) CurrentWindowIndex = 2;
    }

    public async Task LoadDataAsync()
    {
        using var context = await _dbFactory.CreateDbContextAsync();
        if (!Departments.Any())
        {
            var departmentsList = await context.Departments.ToListAsync();
            foreach (var dept in departmentsList) Departments.Add(dept);
            SelectedDepartmentInput = Departments.FirstOrDefault();
        }

        Contacts.Clear();
        var query = context.Contacts.Include(c => c.Department).Where(c => c.IsActive);

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            string search = SearchText.ToLower();
            query = query.Where(c => c.FullName.ToLower().Contains(search) || c.Email.ToLower().Contains(search));
        }

        var contactsList = await query.ToListAsync();
        foreach (var contact in contactsList) Contacts.Add(contact);
    }

    public async Task SaveContactAsync()
    {
        if (!IsFormValid) return;

        using var context = await _dbFactory.CreateDbContextAsync();

        if (CurrentWindowIndex == 1)
        {
            var newContact = new Contact
            {
                FullName = FullNameInput,
                Email = EmailInput,
                Phone = PhoneInput,
                IsActive = true,
                DepartmentId = SelectedDepartmentInput?.Id,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                context.Contacts.Add(newContact);
                await context.SaveChangesAsync();
                await LoadDataAsync();
                ClearForm();
                CurrentWindowIndex = 0;
            }
            catch (DbUpdateException) { }
        }
        else if (CurrentWindowIndex == 2 && SelectedContact != null) 
        {
            var contactInDb = await context.Contacts.FindAsync(SelectedContact.Id);
            if (contactInDb != null)
            {
                contactInDb.FullName = FullNameInput;
                contactInDb.Email = EmailInput;
                contactInDb.Phone = PhoneInput;
                contactInDb.DepartmentId = SelectedDepartmentInput?.Id;

                try
                {
                    await context.SaveChangesAsync();
                    ClearForm();
                    await LoadDataAsync();
                    CurrentWindowIndex = 0;
                }
                catch (DbUpdateException) { }
            }
        }
    }

    public async Task ConfirmDeleteContactAsync()
    {
        if (SelectedContact == null) return;

        using var context = await _dbFactory.CreateDbContextAsync();
        var contactInDb = await context.Contacts.FindAsync(SelectedContact.Id);
        if (contactInDb != null)
        {
            contactInDb.IsActive = false;
            await context.SaveChangesAsync();
            await LoadDataAsync();
            ClearForm();
            CurrentWindowIndex = 0;
        }
    }

    public void ClearForm()
    {
        FullNameInput = string.Empty;
        EmailInput = string.Empty;
        PhoneInput = string.Empty;
        SelectedDepartmentInput = Departments.FirstOrDefault();
        SelectedContact = null;
    }
}

