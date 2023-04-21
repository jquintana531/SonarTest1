using Google.Protobuf.WellKnownTypes;
using MTC.WebApp.BackOffice.Helpers;
using MTC.WebApp.BackOffice.Models;
using MTCenter.GRPC.GDBBO.DBProtos;
using System;

namespace MTC.WebApp.BackOffice.DataManager
{
    public class LeadsManager
    {
        private DBService _client;

        public LeadsManager(DBData.DBDataClient client)
        {
            this._client = new DBService(client);

        }


        #region Listado Leads

        public DataRequestReply GetListadoLeadsEjecutivo(LeadsRequestListadoLeads request)
        {

            DataRequestModel data = new DataRequestModel();
            DataRequestReply reply = new DataRequestReply();

            try
            {
                data.ProcedureName = "leadsv4_listadoMisLeads";
                data.TransactionDb = TransactionDb.Select;

                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "nombre",
                    FieldType = SqlDbType.VarChar,
                    FieldLength = 50,
                    Value = Value.ForString(request.Nombre)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "correo",
                    FieldType = SqlDbType.VarChar,
                    FieldLength = 50,
                    Value = Value.ForString(request.Correo)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "telefono",
                    FieldType = SqlDbType.VarChar,
                    FieldLength = 50,
                    Value = Value.ForString(request.Telefono)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "idFase",
                    FieldType = SqlDbType.Int,
                    FieldLength = 11,
                    Value = Value.ForNumber(request.IdFase)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "idUsuario",
                    FieldType = SqlDbType.Int,
                    FieldLength = 11,
                    Value = Value.ForNumber(request.IdUsuario)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "FIni",
                    FieldType = SqlDbType.VarChar,
                    FieldLength = 10,
                    Value = Value.ForString(request.FechaInicio)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "FFin",
                    FieldType = SqlDbType.VarChar,
                    FieldLength = 10,
                    Value = Value.ForString(request.FechaFin)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "idCadena",
                    FieldType = SqlDbType.Int,
                    FieldLength = 11,
                    Value = Value.ForNumber(request.IdCadena)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "filtroTarea",
                    FieldType = SqlDbType.Int,
                    FieldLength = 11,
                    Value = Value.ForNumber(request.FiltroTarea)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "flag_first",
                    FieldType = SqlDbType.Int,
                    FieldLength = 11,
                    Value = Value.ForNumber(request.PrimeraCarga == true ? 1 : 0)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "PageNumber",
                    FieldType = SqlDbType.Int,
                    FieldLength = 11,
                    Value = Value.ForNumber(request.Pagina)
                });

                reply = _client.GetData(data);

            }
            catch (Exception ex)
            {

                reply.CodigoRespuesta = -109;
                reply.MensajeRespuesta = ex.Message;

            }

            return reply;


        }

        public DataRequestReply GetListadoLeadsJefe(LeadsRequestListadoLeads request)
        {

            DataRequestModel data = new DataRequestModel();
            DataRequestReply reply = new DataRequestReply();

            try
            {
                data.ProcedureName = "leadsv4_listadoLeadsJefe";
                data.TransactionDb = TransactionDb.Select;

               
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "nombre",
                    FieldType = SqlDbType.VarChar,
                    FieldLength = 50,
                    Value = Value.ForString(request.Nombre)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "correo",
                    FieldType = SqlDbType.VarChar,
                    FieldLength = 50,
                    Value = Value.ForString(request.Correo)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "telefono",
                    FieldType = SqlDbType.VarChar,
                    FieldLength = 50,
                    Value = Value.ForString(request.Telefono)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "idFase",
                    FieldType = SqlDbType.Int,
                    FieldLength = 11,
                    Value = Value.ForNumber(request.IdFase)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "idUsuario",
                    FieldType = SqlDbType.Int,
                    FieldLength = 11,
                    Value = Value.ForNumber(request.IdUsuario)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "FIni",
                    FieldType = SqlDbType.VarChar,
                    FieldLength = 10,
                    Value = Value.ForString(request.FechaInicio)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "FFin",
                    FieldType = SqlDbType.VarChar,
                    FieldLength = 10,
                    Value = Value.ForString(request.FechaFin)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "idCadena",
                    FieldType = SqlDbType.Int,
                    FieldLength = 11,
                    Value = Value.ForNumber(request.IdCadena)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "filtroTarea",
                    FieldType = SqlDbType.Int,
                    FieldLength = 11,
                    Value = Value.ForNumber(request.FiltroTarea)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "PageNumber",
                    FieldType = SqlDbType.Int,
                    FieldLength = 11,
                    Value = Value.ForNumber(request.Pagina)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "idCanal",
                    FieldType = SqlDbType.Int,
                    FieldLength = 11,
                    Value = Value.ForNumber(request.IdCanal)
                });

                reply = _client.GetData(data);

            }
            catch (Exception ex)
            {

                reply.CodigoRespuesta = -109;
                reply.MensajeRespuesta = ex.Message;

            }

            return reply;

        }
        #endregion
    }
}
