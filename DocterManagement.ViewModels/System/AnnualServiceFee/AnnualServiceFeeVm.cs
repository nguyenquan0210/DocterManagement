﻿using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Catalog.MasterData;
using DoctorManagement.ViewModels.System.Doctors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.AnnualServiceFee
{
    public class AnnualServiceFeeVm
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public StatusAppointment Status { get; set; }
        public decimal NeedToPay { get; set; }
        public decimal TuitionPaidFreeNumBer { get; set; }
        public string TuitionPaidFreeText { get; set; }
        public decimal Contingency { get; set; }
        public string AccountBank { get; set; }
        public string Type { get; set; }
        public string Image { get; set; }
        public string Note { get; set; }
        public string TransactionCode { get; set; }
        public DateTime PaidDate { get; set; }

        public DoctorVm Doctor { get; set; }
        public string No { get; set; }
        public InformationVm Information { get; set; }
        public string CancelReason { get; set; }
    }
}