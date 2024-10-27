using Blazorise;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Pusula.Training.HealthCare.Employees;
using Pusula.Training.HealthCare.Permissions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages
{
    public partial class Employees
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = [];
        protected PageToolbar Toolbar { get; } = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<EmployeeDto> EmployeeList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateEmployee { get; set; }
        private bool CanEditEmployee { get; set; }
        private bool CanDeleteEmployee { get; set; }
        private EmployeeCreateDto NewEmployee { get; set; }
        private Validations NewEmployeeValidations { get; set; } = new();
        private EmployeeUpdateDto EditingEmployee { get; set; }
        private Validations EditingEmployeeValidations { get; set; } = new();
        private Guid EditingEmployeeId { get; set; }
        private Modal CreateEmployeeModal { get; set; } = new();
        private Modal EditEmployeeModal { get; set; } = new();
        private GetEmployeeInput Filter { get; set; }
        private DataGridEntityActionsColumn<EmployeeDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "Employee-create-tab";
        protected string SelectedEditTab = "Employee-edit-tab";

        private List<EmployeeDto> SelectedEmployees { get; set; } = [];
        private bool AllEmployeesSelected { get; set; }

        public Employees()
        {
            NewEmployee = new EmployeeCreateDto();
            EditingEmployee = new EmployeeUpdateDto();
            Filter = new GetEmployeeInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            EmployeeList = [];


        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {

                await SetBreadcrumbItemsAsync();
                await SetToolbarItemsAsync();
                await InvokeAsync(StateHasChanged);
            }
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Employees"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], DownloadAsExcelAsync, IconName.Download);

            Toolbar.AddButton(L["NewEmployee"], OpenCreateEmployeeModalAsync, IconName.Add, requiredPolicyName: HealthCarePermissions.Employees.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateEmployee = await AuthorizationService
                            .IsGrantedAsync(HealthCarePermissions.Employees.Create);
            CanEditEmployee = await AuthorizationService
                            .IsGrantedAsync(HealthCarePermissions.Employees.Edit);
            CanDeleteEmployee = await AuthorizationService
                            .IsGrantedAsync(HealthCarePermissions.Employees.Delete);


        }

        private async Task GetEmployeesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            
            var result = await EmployeeAppService.GetListAsync(Filter);
            EmployeeList = (IReadOnlyList<EmployeeDto>)result.Items;
            TotalCount = (int)result.TotalCount;

            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetEmployeesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await EmployeeAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HealthCare") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if (!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/Employees/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&FirstName={HttpUtility.UrlEncode(Filter.FirstName)}&LastName={HttpUtility.UrlEncode(Filter.LastName)}&BirthDateMin={Filter.BirthDateMin?.ToString("O")}&BirthDateMax={Filter.BirthDateMax?.ToString("O")}&IdentityNumber={HttpUtility.UrlEncode(Filter.IdentityNumber)}&EmailAddress={HttpUtility.UrlEncode(Filter.Email)}&MobilePhoneNumber={HttpUtility.UrlEncode(Filter.MobilePhoneNumber)}&HomePhoneNumber={HttpUtility.UrlEncode(Filter.HomePhoneNumber)}&GenderMin={Filter.GenderMin}&GenderMax={Filter.GenderMax}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<EmployeeDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetEmployeesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateEmployeeModalAsync()
        {
            NewEmployee = new EmployeeCreateDto
            {
                BirthDate = DateTime.Now,


            };

            SelectedCreateTab = "Employee-create-tab";


            await NewEmployeeValidations.ClearAll();
            await CreateEmployeeModal.Show();
        }

        private async Task CloseCreateEmployeeModalAsync()
        {
            NewEmployee = new EmployeeCreateDto
            {
                BirthDate = DateTime.Now,


            };
            await CreateEmployeeModal.Hide();
        }

        private async Task OpenEditEmployeeModalAsync(EmployeeDto input)
        {
            SelectedEditTab = "Employee-edit-tab";


            var Employee = await EmployeeAppService.GetAsync(input.Id);

            EditingEmployeeId = Employee.Id;
            EditingEmployee = ObjectMapper.Map<EmployeeDto, EmployeeUpdateDto>(Employee);

            await EditingEmployeeValidations.ClearAll();
            await EditEmployeeModal.Show();
        }

        private async Task DeleteEmployeeAsync(EmployeeDto input)
        {
            await EmployeeAppService.DeleteAsync(input.Id);
            await GetEmployeesAsync();
        }

        private async Task CreateEmployeeAsync()
        {
            try
            {
                if (await NewEmployeeValidations.ValidateAll() == false)
                {
                    return;
                }

                await EmployeeAppService.CreateAsync(NewEmployee);
                await GetEmployeesAsync();
                await CloseCreateEmployeeModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditEmployeeModalAsync()
        {
            await EditEmployeeModal.Hide();
        }

        private async Task UpdateEmployeeAsync()
        {
            try
            {
                if (await EditingEmployeeValidations.ValidateAll() == false)
                {
                    return;
                }

                await EmployeeAppService.UpdateAsync(EditingEmployeeId, EditingEmployee);
                await GetEmployeesAsync();
                await EditEmployeeModal.Hide();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        protected virtual async Task OnFirstNameChangedAsync(string? firstName)
        {
            Filter.FirstName = firstName;
            await SearchAsync();
        }
        protected virtual async Task OnLastNameChangedAsync(string? lastName)
        {
            Filter.LastName = lastName;
            await SearchAsync();
        }
        protected virtual async Task OnBirthDateMinChangedAsync(DateTime? birthDateMin)
        {
            Filter.BirthDateMin = birthDateMin.HasValue ? birthDateMin.Value.Date : birthDateMin;
            await SearchAsync();
        }
        protected virtual async Task OnBirthDateMaxChangedAsync(DateTime? birthDateMax)
        {
            Filter.BirthDateMax = birthDateMax.HasValue ? birthDateMax.Value.Date.AddDays(1).AddSeconds(-1) : birthDateMax;
            await SearchAsync();
        }
        protected virtual async Task OnIdentityNumberChangedAsync(string? identityNumber)
        {
            Filter.IdentityNumber = identityNumber;
            await SearchAsync();
        }
        protected virtual async Task OnEmailAddressChangedAsync(string? email)
        {
            Filter.Email = email;
            await SearchAsync();
        }
        protected virtual async Task OnMobilePhoneNumberChangedAsync(string? mobilePhoneNumber)
        {
            Filter.MobilePhoneNumber = mobilePhoneNumber;
            await SearchAsync();
        }
        private Task SelectAllItems()
        {
            AllEmployeesSelected = true;

            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllEmployeesSelected = false;
            SelectedEmployees.Clear();

            return Task.CompletedTask;
        }

        private Task SelectedEmployeeRowsChanged()
        {
            if (SelectedEmployees.Count != PageSize)
            {
                AllEmployeesSelected = false;
            }

            return Task.CompletedTask;
        }

        private async Task DeleteSelectedEmployeesAsync()
        {
            var message = AllEmployeesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedEmployees.Count].Value;

            /*if (!await IUiMessageService.Confirm(message))
            {
                return;
            }*/

            if (AllEmployeesSelected)
            {
                await EmployeeAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await EmployeeAppService.DeleteByIdsAsync(SelectedEmployees.Select(x => x.Id).ToList());
            }

            SelectedEmployees.Clear();
            AllEmployeesSelected = false;

            await GetEmployeesAsync();
        }
    }
}
