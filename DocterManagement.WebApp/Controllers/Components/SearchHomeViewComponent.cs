using DoctorManagement.ApiIntegration;
using DoctorManagement.ViewModels.Catalog.Appointment;
using DoctorManagement.ViewModels.System.Doctors;
using DoctorManagement.ViewModels.System.Statistic;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.WebApp.Controllers.Components
{
    public class SearchHomeViewComponent : ViewComponent
    {
        private readonly IAppointmentApiClient _appointmentApiClient;
        private readonly IMasterDataApiClient _masterDataApiClient;
        private readonly IStatisticApiClient _statisticApiClient;
        public SearchHomeViewComponent(IMasterDataApiClient masterDataApiClient,
            IAppointmentApiClient appointmentApiClient,IStatisticApiClient statisticApiClient)
        {
            _masterDataApiClient = masterDataApiClient;
            _appointmentApiClient = appointmentApiClient;
            _statisticApiClient = statisticApiClient;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var appointments = new List<AppointmentVm>();
            if (User.Identity.Name != null)
            {
                var request = new GetAppointmentPagingRequest()
                {
                    PageIndex = 1,
                    PageSize = 100,
                    UserName = User.Identity.Name,
                    status = Data.Enums.StatusAppointment.complete
                };
                appointments = (await _appointmentApiClient.GetAppointmentPagings(request)).Data.Items.Where(x=>x.Doctor.Booking == true).ToList();

            }
            var checktring = "";
            var docters = new List<DoctorVm>();
            foreach (var appointment in appointments)
            {
                if (checktring.Contains(appointment.Doctor.UserId.ToString()))
                {
                    var doctor = new DoctorVm()
                    {
                        No = appointment.Doctor.No,
                        FirstName = appointment.Doctor.FirstName,
                        LastName = appointment.Doctor.LastName,
                        Img = appointment.Doctor.Img,
                    };
                    docters.Add(doctor);
                }
                checktring = checktring + "," + appointment.Doctor.UserId.ToString();
            }
            ViewBag.Doctor = docters;
            var mainMenus = (await _masterDataApiClient.GetAllMainMenu()).Data;
            ViewBag.MenuPanner = mainMenus.Where(x => x.Type == "MenuPanner").ToList();
            ViewBag.Keywords = (await _statisticApiClient.ListActiveUserDetailt()).Where(x=>x.ServiceName.Contains("FilterDoctorHome")).DistinctBy(x=>x.Parameters).ToList();
            return View();
        }

    }
}
