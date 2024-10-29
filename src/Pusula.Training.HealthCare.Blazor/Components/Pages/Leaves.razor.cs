using Blazorise;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Leaves;
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
using Pusula.Training.HealthCare.Employees;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages
{
    public partial class Leaves
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = [];
        protected PageToolbar Toolbar { get; } = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<LeaveWithNavigationPropertiesDto> LeaveList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateLeave { get; set; }
        private bool CanEditLeave { get; set; }
        private bool CanDeleteLeave { get; set; }
        private LeaveCreateDto NewLeave { get; set; }
        private Validations NewLeaveValidations { get; set; } = new();
        private LeaveUpdateDto EditingLeave { get; set; }
        private Validations EditingLeaveValidations { get; set; } = new();
        private Guid EditingLeaveId { get; set; }
        private Modal CreateLeaveModal { get; set; } = new();
        private Modal EditLeaveModal { get; set; } = new();
        private GetLeavesInput Filter { get; set; }
        private DataGridEntityActionsColumn<LeaveWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "leave-create-tab";
        protected string SelectedEditTab = "leave-edit-tab";

        private IReadOnlyList<LookupDto<LeaveType>> LeaveTypesCollection { get; set; } = new List<LookupDto<LeaveType>>();
        private IReadOnlyList<LookupDto<Guid>> EmployeesCollection { get; set; } = [];
        private List<LeaveWithNavigationPropertiesDto> SelectedLeave { get; set; } = [];
        private bool AllLeavesSelected { get; set; }

        public Leaves()
        {
            NewLeave = new LeaveCreateDto();
            EditingLeave = new LeaveUpdateDto();
            Filter = new GetLeavesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting

            };
            LeaveList = [];
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetEmployeeCollectionLookupAsync();

            LeaveTypesCollection = Enum.GetValues(typeof(LeaveType))
           .Cast<LeaveType>()
           .Select(g => new LookupDto<LeaveType>
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Leaves"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], DownloadAsExcelAsync, IconName.Download);

            Toolbar.AddButton(L["NewLeave"], OpenCreateLeaveModalAsync, IconName.Add, requiredPolicyName: HealthCarePermissions.Leaves.Create);

            return ValueTask.CompletedTask;
        }
        private async Task SetPermissionsAsync()
        {
            CanCreateLeave = await AuthorizationService
                .IsGrantedAsync(HealthCarePermissions.Leaves.Create);
            CanEditLeave = await AuthorizationService
                            .IsGrantedAsync(HealthCarePermissions.Leaves.Edit);
            CanDeleteLeave = await AuthorizationService
                            .IsGrantedAsync(HealthCarePermissions.Leaves.Delete);

        }
        private async Task GetLeavesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await LeaveAppService.GetListAsync(Filter);
            LeaveList = result.Items;
            TotalCount = (int)result.TotalCount;

            await ClearSelection();
        }
        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetLeavesAsync();
            await InvokeAsync(StateHasChanged);
        }
        private async Task DownloadAsExcelAsync()
        {
            var token = (await LeaveAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HealthCare") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if (!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/leaves/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Description={HttpUtility.UrlEncode(Filter.Description)}&EmployeeId={Filter.EmployeeId}", forceLoad: true);
        }
        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<LeaveWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetLeavesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateLeaveModalAsync()
        {
            NewLeave = new LeaveCreateDto
            {
                StartDate = DateTime.Now,
                EndDate= DateTime.Now,
                EmployeeId = EmployeesCollection.Select(x => x.Id).FirstOrDefault()
            };

            SelectedCreateTab = "leave-create-tab";

            await NewLeaveValidations.ClearAll();
            await CreateLeaveModal.Show();
        }

        private async Task CloseCreateLeaveModalAsync()
        {
            NewLeave = new LeaveCreateDto
            {
                StartDate = DateTime.Now,
                EmployeeId = EmployeesCollection.Select(x => x.Id).FirstOrDefault()
            };

            await CreateLeaveModal.Hide();
        }

        private async Task OpenEditLeaveModalAsync(LeaveWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "leave-edit-tab";


            var leave = await LeaveAppService.GetWithNavigationPropertiesAsync(input.Leave.Id);

            EditingLeaveId = leave.Leave.Id;
            EditingLeave = ObjectMapper.Map<LeaveDto, LeaveUpdateDto>(leave.Leave);

            await EditingLeaveValidations.ClearAll();
            await EditLeaveModal.Show();
        }

        private async Task DeleteLeaveAsync(LeaveWithNavigationPropertiesDto input)
        {
            await LeaveAppService.DeleteAsync(input.Leave.Id);
            await GetLeavesAsync();
        }

        private async Task CreateLeaveAsync()
        {
            try
            {
                if (await NewLeaveValidations.ValidateAll() == false)
                {
                    return;
                }

                await LeaveAppService.CreateAsync(NewLeave);
                await GetLeavesAsync();
                await CloseCreateLeaveModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditLeaveModalAsync()
        {
            await EditLeaveModal.Hide();
        }

        private async Task UpdateLeaveAsync()
        {
            try
            {
                if (await EditingLeaveValidations.ValidateAll() == false)
                {
                    return;
                }

                await LeaveAppService.UpdateAsync(EditingLeave);
                await GetLeavesAsync();
                await EditLeaveModal.Hide();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        protected virtual async Task OnStartDateChangedAsync(DateTime? startDate)
        {
            Filter.StartDate = startDate.HasValue ? startDate.Value.Date : startDate;
            await SearchAsync();
        }
        protected virtual async Task OnEndDateChangedAsync(DateTime? endDate)
        {
            Filter.EndDate = endDate.HasValue ? endDate.Value.Date : endDate;
            await SearchAsync();
        }
        protected virtual async Task OnLeaveTypeChangedAsync(LeaveType? leaveType)
        {
            Filter.LeaveType = leaveType;
            await SearchAsync();
        }
        protected virtual async Task OnDescriptionChangedAsync(string description)
        {
            Filter.Description = description;
            await SearchAsync();
        }
        protected virtual async Task OnEmployeeIdChangedAsync(Guid? employeeId)
        {
            Filter.EmployeeId = employeeId;
            await SearchAsync();
        }
        private async Task GetEmployeeCollectionLookupAsync(string? newValue = null)
        {
            EmployeesCollection = (await LeaveAppService.GetEmployeeLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private Task SelectAllItems()
        {
            AllLeavesSelected = true;

            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllLeavesSelected = false;
            SelectedLeave.Clear();

            return Task.CompletedTask;
        }

        private Task SelectedLeaveRowsChanged()
        {
            if (SelectedLeave.Count != PageSize)
            {
                AllLeavesSelected = false;
            }

            return Task.CompletedTask;
        }
        private async Task DeleteSelectedLeavesAsync()
        {
            var message = AllLeavesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedLeave.Count].Value;

            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllLeavesSelected)
            {
                await LeaveAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await LeaveAppService.DeleteByIdsAsync(SelectedLeave.Select(x => x.Leave.Id).ToList());
            }

            SelectedLeave.Clear();
            AllLeavesSelected = false;

            await GetLeavesAsync();
        }

    }

}
