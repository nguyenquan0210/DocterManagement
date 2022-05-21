using DoctorManagement.ApiIntegration;
using DoctorManagement.Utilities.Constants;
using DoctorManagement.ViewModels.System.Patient;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DoctorManagement.WebApp.Controllers.Components
{
    public class UserInfoViewComponent : ViewComponent
    {
        private readonly IDoctorApiClient _doctorApiClient;
        public UserInfoViewComponent(IDoctorApiClient doctorApiClient)
        {
            _doctorApiClient = doctorApiClient;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //if(User.Identity.Name==null) return View();
            var patients = (await _doctorApiClient.GetPatientProfile("0373951042")).Data;
            var patient = patients.FirstOrDefault(x => x.IsPrimary == true);
            ViewBag.PatientName = patient.Name;
            return View();
        }

    }
}
