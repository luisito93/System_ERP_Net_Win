﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ELRConsultasReportes
{
    public partial class FormActivosFijosPorOficinas : WinControl_ELR_NET.FormPlantillaIndex
    {
        public FormActivosFijosPorOficinas()
        {
            InitializeComponent();
        }

        void GetOficina(string codigo = "")
        {
            DataTable DTData = null;
            DataRow fila0 = null;
            string SSQL = "", id = "0", nombre = "(TODOS)";
            ELRMaestros.FormOficinas frm = new ELRMaestros.FormOficinas();

            try
            {
                codigo = codigo.Trim().Replace('-', ' ');
                if (codigo != "")
                {
                    SSQL = " SELECT TOP 1 Oficina_Id, Nombre FROM TEMPRESAS_OFICINAS";
                    SSQL += " WHERE Cast(Oficina_Id as Varchar) = '" + codigo + "' ";
                    SSQL += "  And Empresa_Id = " + empresaID.ToString();

                    DTData = objDB.GetSQL(SSQL);
                    if (DTData.Rows.Count > 0) fila0 = DTData.Rows[0];
                }
                else
                {
                    frm.empresaID = empresaID;
                    frm.oficinaID = oficinaID;
                    frm.EsAgregar = false;
                    frm.EsModificar = false;
                    frm.esBusqueda = true;
                    frm.EnEjecusion = true;
                    frm.FiltroEstatico = "Empresa_Id = " + empresaID.ToString();


                    frm.ShowDialog();

                    if (frm.filaSelecionada != null) fila0 = frm.filaSelecionada;

                }

                if (fila0 != null)
                {
                    id = objUtil.GetAsString("Oficina_Id", fila0);
                    nombre = objUtil.GetAsString("Nombre", fila0);
                }

                txtCodigo.Text = id;
                txtNombre.Text = nombre;
                toolTip1.SetToolTip(txtNombre, nombre);

                GetData();
            }
            catch (Exception ex)
            {

                objUtil.MostrarMensajeError(ex.Message);
            }
        }

        void GetTipo(string codigo = "")
        {
            DataTable DTData = null;
            DataRow fila0 = null;
            string SSQL = "", id = "0", nombre = "(TODOS)";
            ELRActivosFijos.FormActivosFijosTipos frm = new ELRActivosFijos.FormActivosFijosTipos();

            try
            {
                codigo = codigo.Trim().Replace('-', ' ');
                if (codigo != "")
                {
                    SSQL = " SELECT TOP 1 Tipo_Id, Descripcion FROM TACTIVOS_FIJOS_TIPOS";
                    SSQL += " WHERE Cast(Tipo_Id as Varchar) = '" + codigo + "' ";

                    DTData = objDB.GetSQL(SSQL);
                    if (DTData.Rows.Count > 0) fila0 = DTData.Rows[0];
                }
                else
                {
                    frm.empresaID = empresaID;
                    frm.oficinaID = oficinaID;
                    frm.EsAgregar = false;
                    frm.EsModificar = false;
                    frm.esBusqueda = true;
                    frm.EnEjecusion = true;
                   
                    frm.ShowDialog();

                    if (frm.filaSelecionada != null) fila0 = frm.filaSelecionada;

                }

                if (fila0 != null)
                {
                    id = objUtil.GetAsString("Tipo_Id", fila0);
                    nombre = objUtil.GetAsString("Descripcion", fila0);
                }

                txtTipo.Text = id;
                txtNombreTipo.Text = nombre;
                toolTip1.SetToolTip(txtNombreTipo, nombre);

                GetData();
            }
            catch (Exception ex)
            {

                objUtil.MostrarMensajeError(ex.Message);
            }
        }

        public override void GetData(int opcion = 0)
        {
            int id = 0;
            int fechaDesde = 0, fechaHasta = 0;
            double valorAdquisicion = 0, depreAcumulada = 0, valorLibro = 0;
            try
            {
                Cursor = Cursors.WaitCursor;

                fechaDesde = objUtil.DateToInt(dtpFecha1.Value);
                fechaHasta = objUtil.DateToInt(dtpFecha2.Value);

                objDB.LimpiarFiltros();
                objDB.AddFiltroIgualA("EsActivo", "1");
                objDB.AddFiltroIsNull("Fecha_Baja");

                int.TryParse(txtCodigo.Text, out id);
                if (id > 0) objDB.AddFiltroIgualA("Oficina_Id", id.ToString());

                id = 0;
                int.TryParse(txtTipo.Text, out id);
                if (id > 0) objDB.AddFiltroIgualA("Tipo_Id", id.ToString());

                if(chkPorFecha.Checked)
                {
                    objDB.AddFiltroMayorOIgualA("Fecha_Adquisicion", fechaDesde.ToString());
                    objDB.AddFiltroMenorOIgualA("Fecha_Adquisicion", fechaHasta.ToString());
                }

                MyData = objDB.GetAll("VACTIVOS_FIJOS", -1, objDB.Filtros);
                DataGridConsulta.AutoGenerateColumns = false;
                DataGridConsulta.DataSource = MyData.DefaultView;
                MostrarCantidadFilas();

                valorAdquisicion = GetSum("Valor_Adquisicion");
                depreAcumulada = GetSum("Depre_Acumulada");
                valorLibro = GetSum("Valor_Libro_Depreciar");

                lblCantidadRegistro.Text = "Valor Adquisicion: " + valorAdquisicion.ToString("N2");
                lblCantidadRegistro.Text += "   -Depre. Acum.: " + depreAcumulada.ToString("N2");
                lblCantidadRegistro.Text += "    =Valor En Libro: " + valorLibro.ToString("N2");

                toolTip1.SetToolTip(lblCantidadRegistro, lblCantidadRegistro.Text);

                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {

                objUtil.MostrarMensajeError(ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void FormActivosFijosPorOficinas_Load(object sender, EventArgs e)
        {
            DateTime fecha = DateTime.Now.Date;
            dtpFecha2.Value = fecha;
            dtpFecha1.Value = fecha.AddMonths(-1);

            GetData();
        }

        private void btnBuscarOficina_Click(object sender, EventArgs e)
        {
            GetOficina();
        }

        private void btnBuscarTipo_Click(object sender, EventArgs e)
        {
            GetTipo();
        }

        private void txtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            string codigo = txtCodigo.Text.Trim();
            if (e.KeyCode == Keys.F9) GetOficina();
            else if (e.KeyCode == Keys.Enter && codigo != "") GetOficina(codigo);
        }

        private void txtTipo_KeyDown(object sender, KeyEventArgs e)
        {
            string codigo = txtTipo.Text.Trim();
            if (e.KeyCode == Keys.F9) GetTipo();
            else if (e.KeyCode == Keys.Enter && codigo != "") GetTipo(codigo);
        }

        public override void Imprimir()
        {
            WinControl_ELR_NET.ELRFormPreviewRDLC frm = new WinControl_ELR_NET.ELRFormPreviewRDLC();
            string pathReporte = "";
            DateTime fecha = DateTime.Now.Date;
            try
            {
                pathReporte = objUtil.GetPathReportRDLC("RptActivosFijosListado");

                frm.pathReporte = pathReporte;
                frm.DTData = MyData;
                frm.DTOficina = DTOficina;
                frm.oficinaId = oficinaID;
                frm.empresaId = empresaID;
                frm.nombreUsuario = nombreUsuario;

                if(!chkPorFecha.Checked) frm.titulo = "LISTADO DE ACTIVOS FIJOS POR OFICINAS A FECHA: " + objUtil.GetDateAsString(fecha);
                else frm.titulo = "LISTADO DE ACTIVOS FIJOS POR OFICINAS, FECHA ADQUISICION: [ " + objUtil.GetDateAsString(dtpFecha1.Value) + " - " + objUtil.GetDateAsString(dtpFecha2.Value) + "]";

                frm.ShowDialog();
            }
            catch (Exception ex)
            {

                objUtil.MostrarMensajeError(ex.Message);
            }
            finally
            {
                frm.Dispose();
            }
        }

        private void chkPorFecha_CheckedChanged(object sender, EventArgs e)
        {
            dtpFecha1.Enabled = chkPorFecha.Checked;
            dtpFecha2.Enabled = chkPorFecha.Checked;

            GetData();
        }
    }
}