﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ELRNomina
{
    public partial class FormVacacionesR : WinControl_ELR_NET.ELRFormRegistro
    {
        public DataRow fila0;
        string empleadoId = "0";
        string nombreCompleto = "";

        public FormVacacionesR()
        {
            InitializeComponent();
        }

        private void FormVacacionesR_Load(object sender, EventArgs e)
        {
            DateTime fecha = DateTime.Now.Date;
            dtpFecha.Value = fecha;
            dtpFechaRetornar.Value = fecha.AddDays(16);
        }

        public override bool GetData()
        {
            //return base.GetData();
            bool resultado = false;
            string cedula = "";

            try
            {
                empleadoId = objUtil.GetAsString("Empleado_Id", fila0);
                cedula = objUtil.GetAsString("CedulaRNC", fila0);
                nombreCompleto = objUtil.GetAsString("Nombres", fila0) + " " + objUtil.GetAsString("Apellidos", fila0);

                lblCodigo.Text = "Codigo: " + empleadoId.PadLeft(7, '0') + "              Cedula: " + cedula;
                lblNombreCompleto.Text = nombreCompleto.ToUpper();
            }
            catch (Exception ex)
            {

                objUtil.MostrarMensajeError(ex.Message);
            }


            return resultado;
        }

        public override int GrabarDatos()
        {
            //return base.GrabarDatos();
            int resultado = -1;
            DialogResult resp;
            List<DataDB_ELR_NET.Parametro> parms = new List<DataDB_ELR_NET.Parametro>();
            int fechaId = 0, fechaRetornar;
            string concepto = "";
            int spReturn = 0;

            try
            {
                fechaId = objUtil.DateToInt(dtpFecha.Value);
                fechaRetornar = objUtil.DateToInt(dtpFechaRetornar.Value);
                concepto = txtMotivo.Text.Trim();

                resp = objUtil.MostrarMensajePregunta("Esta Seguro que desea Aplicar Vacaciones A: " + nombreCompleto + "?");

                if (resp == DialogResult.Yes)
                {
                    Cursor = Cursors.WaitCursor;

                    if (temporalID == "") temporalID = objUtil.GetTemporalID();

                    parms.Add(new DataDB_ELR_NET.Parametro("IdentificadorTemp", temporalID));
                    parms.Add(new DataDB_ELR_NET.Parametro("empleadoId", empleadoId));
                    parms.Add(new DataDB_ELR_NET.Parametro("Fecha", fechaId));
                    parms.Add(new DataDB_ELR_NET.Parametro("FechaARetornar", fechaRetornar));
                    parms.Add(new DataDB_ELR_NET.Parametro("Concepto", concepto));
                    parms.Add(new DataDB_ELR_NET.Parametro("Autor", nombreUsuario));

                    objDB.iniciarTransaccion();
                    spReturn = objDB.EjecutarSP("SP_RRHH_REGISTRAR_VACACIONES_AEMPLEADO", parms);
                    if (spReturn <= 0)
                    {
                        MostrarErrorTemporal();
                        objDB.cancelarTransaccion();
                    }
                    else
                    {
                        objDB.confirmarTransaccion();
                        lOK = true;
                        Close();
                    }
                }
            }
            catch (Exception ex)
            {

                objUtil.MostrarMensajeError(ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
                objDB.cancelarTransaccion();
            }

            return resultado;
        }
    }
}
