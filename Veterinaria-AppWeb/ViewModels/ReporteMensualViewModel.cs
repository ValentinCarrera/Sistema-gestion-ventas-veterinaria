using System;
using System.Collections.Generic;

namespace Veterinaria_AppWeb.ViewModels
{
    public class ReporteMensualViewModel
    {
        public int Año { get; set; }
        public int Mes { get; set; }

        public List<ReporteDia> DetallePorDia { get; set; } = new();
        public decimal TotalMes { get; set; }
        public decimal TotalEfectivo { get; set; }
        public decimal TotalDebito { get; set; }
        public decimal TotalCredito { get; set; }
        public decimal TotalTransferencia { get; set; }
    }

    public class ReporteDia
    {
        public DateTime Dia { get; set; }
        public decimal Total { get; set; }
        public decimal Efectivo { get; set; }
        public decimal Debito { get; set; }
        public decimal Credito { get; set; }
        public decimal Transferencia { get; set; }
    }
}
