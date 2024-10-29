using Blazorise;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Pusula.Training.HealthCare.Employees;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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
        private IReadOnlyList<EmployeeWithNavigationPropertiesDto> EmployeeList { get; set; }
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
        private DataGridEntityActionsColumn<EmployeeWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "employee-create-tab";
        protected string SelectedEditTab = "employee-edit-tab";

        private IReadOnlyList<LookupDto<Guid>> DepartmentsCollection { get; set; } = [];
        private IReadOnlyList<LookupDto<EnumGender>> GendersCollection { get; set; } = new List<LookupDto<EnumGender>>();
        private List<EmployeeWithNavigationPropertiesDto> SelectedEmployee { get; set; } = [];
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
            await GetDepartmentCollectionLookupAsync();
            GendersCollection = Enum.GetValues(typeof(EnumGender))
           .Cast<EnumGender>()
           .Select(g => new LookupDto<EnumGender>
           {
               Id = g,
               DisplayName = g.ToString()
           })
           .ToList();
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
            CanCreateEmployee = await AuthorizationService.IsGrantedAsync(HealthCarePermissions.Employees.Create);
            CanEditEmployee = await AuthorizationService.IsGrantedAsync(HealthCarePermissions.Employees.Edit);
            CanDeleteEmployee = await AuthorizationService.IsGrantedAsync(HealthCarePermissions.Employees.Delete);
        }

        private async Task GetEmployeesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await EmployeeAppService.GetListAsync(Filter);
            EmployeeList = (IReadOnlyList<EmployeeWithNavigationPropertiesDto>)result.Items;
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
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/employees/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Name={HttpUtility.UrlEncode(Filter.FirstName)}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<EmployeeWithNavigationPropertiesDto> e)
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
                BirthDate=DateTime.Now,

                DepartmentId = DepartmentsCollection.Select(x => x.Id).FirstOrDefault()
            };

            SelectedCreateTab = "employee-create-tab";

            await NewEmployeeValidations.ClearAll();
            await CreateEmployeeModal.Show();
        }

        private async Task CloseCreateEmployeeModalAsync()
        {
            NewEmployee = new EmployeeCreateDto
            {

            };

            await CreateEmployeeModal.Hide();
        }

        private async Task OpenEditEmployeeModalAsync(EmployeeWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "employee-edit-tab";

            var employee = await EmployeeAppService.GetAsync(input.Employee.Id);

            EditingEmployeeId = employee.Id;
            EditingEmployee = ObjectMapper.Map<EmployeeDto, EmployeeUpdateDto>(employee);

            await EditingEmployeeValidations.ClearAll();
            await EditEmployeeModal.Show();
        }

        private async Task DeleteEmployeeAsync(EmployeeWithNavigationPropertiesDto input)
        {
            await EmployeeAppService.DeleteAsync(input.Employee.Id);
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
                await CloseEditEmployeeModalAsync();
            }
            catch (Exception ex)
            {

            }
        }

        private void OnSelectedCreateTabChanged(string tabName)
        {
            SelectedCreateTab = tabName;
        }

        private void OnSelectedEditTabChanged(string tabName)
        {
            SelectedEditTab = tabName;
        }

        protected virtual async Task OnFirstNameChangedAsync(string? name)
        {
            Filter.FirstName = name;
            await SearchAsync();
        }
        protected virtual async Task OnLastNameChangedAsync(string? name)
        {
            Filter.LastName = name;
            await SearchAsync();
        }
        protected virtual async Task OnIdentityNumberChangedAsync(string? name)
        {
            Filter.IdentityNumber = name;
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
            SelectedEmployee.Clear();

            return Task.CompletedTask;
        }

        private Task SelectedEmployeeRowsChanged()
        {
            if (SelectedEmployee.Count != PageSize)
            {
                AllEmployeesSelected = false;
            }

            return Task.CompletedTask;
        }

        private async Task GetDepartmentCollectionLookupAsync(string? newValue = null)
        {
            DepartmentsCollection = (await EmployeeAppService.GetDepartmentLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task DeleteSelectedEmployeesAsync()
        {
            var message = AllEmployeesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedEmployee.Count].Value;
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }
            if (AllEmployeesSelected)
            {
                await EmployeeAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await EmployeeAppService.DeleteByIdsAsync(SelectedEmployee.Select(x => x.Employee.Id).ToList());
            }

            SelectedEmployee.Clear();
            AllEmployeesSelected = false;

            await GetEmployeesAsync();
        }
    }
}
