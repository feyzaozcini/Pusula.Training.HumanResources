using Blazorise;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Salaries;
using Pusula.Training.HealthCare.Shared;
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
    public partial class Salaries
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = [];
        protected PageToolbar Toolbar { get; } = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<SalaryWithNavigationPropertiesDto> SalaryList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateSalary { get; set; }
        private bool CanEditSalary { get; set; }
        private bool CanDeleteSalary { get; set; }
        private SalaryCreateDto NewSalary { get; set; }
        private Validations NewSalaryValidations { get; set; } = new();
        private SalaryUpdateDto EditingSalary { get; set; }
        private Validations EditingSalaryValidations { get; set; } = new();
        private Guid EditingSalaryId { get; set; }
        private Modal CreateSalaryModal { get; set; } = new();
        private Modal EditSalaryModal { get; set; } = new();
        private GetSalaryInput Filter { get; set; }
        private DataGridEntityActionsColumn<SalaryWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "Salary-create-tab";
        protected string SelectedEditTab = "Salary-edit-tab";

        private IReadOnlyList<LookupDto<Guid>> EmployeesCollection { get; set; } = [];
        private List<SalaryWithNavigationPropertiesDto> SelectedSalaries { get; set; } = [];

        private bool AllSalariesSelected { get; set; }

        public Salaries()
        {
            NewSalary = new SalaryCreateDto();
            EditingSalary = new SalaryUpdateDto();
            Filter = new GetSalaryInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting

            };
            SalaryList = [];
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetEmployeeCollectionLookupAsync();

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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Salaries"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], DownloadAsExcelAsync, IconName.Download);

            Toolbar.AddButton(L["NewSalary"], OpenCreateSalaryModalAsync, IconName.Add, requiredPolicyName: HealthCarePermissions.Salaries.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateSalary = await AuthorizationService
                .IsGrantedAsync(HealthCarePermissions.Salaries.Create);
            CanEditSalary = await AuthorizationService
                            .IsGrantedAsync(HealthCarePermissions.Salaries.Edit);
            CanDeleteSalary = await AuthorizationService
                            .IsGrantedAsync(HealthCarePermissions.Salaries.Delete);

        }

        private async Task GetSalaryAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await SalaryAppService.GetListAsync(Filter);
            SalaryList = result.Items;
            TotalCount = (int)result.TotalCount;

            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetSalaryAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await SalaryAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HealthCare") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if (!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/salaries/as-excel-file?DownloadToken={token}&EmployeeId={Filter.EmployeeId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<SalaryWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetSalaryAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateSalaryModalAsync()
        {
            NewSalary = new SalaryCreateDto
            {
                BaseAmount = 0,
                Bonus = 0,
                Deduction = 0,
                EffectiveFrom = DateTime.Now,
                EffectiveTo = DateTime.Now,
                EmployeeId = EmployeesCollection.Select(i => i.Id).FirstOrDefault(),

            };

            SelectedCreateTab = "salary-create-tab";


            await NewSalaryValidations.ClearAll();
            await CreateSalaryModal.Show();
        }
        private async Task CloseCreateSalaryModalAsync()
        {
            NewSalary = new SalaryCreateDto
            {
                EffectiveFrom = DateTime.Now,
                EmployeeId = EmployeesCollection.Select(i => i.Id).FirstOrDefault(),

            };
            await CreateSalaryModal.Hide();
        }
        private async Task OpenEditSalaryModalAsync(SalaryWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "salary-edit-tab";


            var salary = await SalaryAppService.GetWithNavigationPropertiesAsync(input.Salary.Id);

            EditingSalaryId = salary.Salary.Id;
            EditingSalary = ObjectMapper.Map<SalaryDto, SalaryUpdateDto>(salary.Salary);

            await EditingSalaryValidations.ClearAll();
            await EditSalaryModal.Show();
        }
        private async Task DeleteSalaryAsync(SalaryWithNavigationPropertiesDto input)
        {
            await SalaryAppService.DeleteAsync(input.Salary.Id);
            await GetSalaryAsync();
        }

        private async Task CreateSalaryAsync()
        {
            try
            {
                if (await NewSalaryValidations.ValidateAll() == false)
                {
                    return;
                }

                await SalaryAppService.CreateAsync(NewSalary);
                await GetSalaryAsync();
                await CloseCreateSalaryModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditSalaryModalAsync()
        {
            await EditSalaryModal.Hide();
        }

        private async Task UpdateSalaryAsync()
        {
            try
            {
                if (await EditingSalaryValidations.ValidateAll() == false)
                {
                    return;
                }

                await SalaryAppService.UpdateAsync(EditingSalary);
                await GetSalaryAsync();
                await EditSalaryModal.Hide();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        protected virtual async Task OnBaseAmount(decimal? baseAmount)
        {
            Filter.BaseAmount = baseAmount;
            await SearchAsync();
        }
        protected virtual async Task OnBonus(decimal? bonus)
        {
            Filter.Bonus = bonus;
            await SearchAsync();
        }
        protected virtual async Task OnDeduction(decimal? deduction)
        {
            Filter.Deduction = deduction;
            await SearchAsync();
        }
        protected virtual async Task OnEffectiveFrom(DateTime? effectiveFrom)
        {
            Filter.EffectiveFrom = effectiveFrom;
            await SearchAsync();
        }
        protected virtual async Task OnEffectiveTo(DateTime? effectiveTo)
        {
            Filter.EffectiveTo = effectiveTo;
            await SearchAsync();
        }
        protected virtual async Task OnEmployeeId(Guid? employeeId)
        {
            Filter.EmployeeId = employeeId;
            await SearchAsync();
        }
        private async Task GetEmployeeCollectionLookupAsync(string? newValue = null)
        {
            EmployeesCollection = (await SalaryAppService.GetEmployeeLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private Task SelectAllItems()
        {
            AllSalariesSelected = true;

            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllSalariesSelected = false;
            SelectedSalaries.Clear();

            return Task.CompletedTask;
        }

        private Task SelectedSalaryRowsChanged()
        {
            if (SelectedSalaries.Count != PageSize)
            {
                AllSalariesSelected = false;
            }

            return Task.CompletedTask;
        }

        private async Task DeleteSelectedSalariesAsync()
        {
            var message = AllSalariesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedSalaries.Count].Value;

            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllSalariesSelected)
            {
                await SalaryAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await SalaryAppService.DeleteByIdsAsync(SelectedSalaries.Select(x => x.Salary.Id).ToList());
            }

            SelectedSalaries.Clear();
            AllSalariesSelected = false;

            await GetSalaryAsync();
        }
    }
}
