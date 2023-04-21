using Google.Protobuf.WellKnownTypes;
using MTC.WebApp.BackOffice.Helpers;
using MTC.WebApp.BackOffice.Models;
using MTCenter.GRPC.GDBBO.DBProtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.WebApp.BackOffice.DataManager
{
    public class LiberacionesManager
    {
        private DBService _client;

        public LiberacionesManager(DBData.DBDataClient client)
        {
            this._client = new DBService(client);

        }

        #region Listado Depositos

        public DataRequestReply GetListadoDepositos(LiberacionesRequestListadoDepositos request)
        {

            DataRequestModel data = new DataRequestModel();
            DataRequestReply reply = new DataRequestReply();

            try
            {
                data.ProcedureName = "sp_getListadoDepositos";
                data.TransactionDb = TransactionDb.Select;

                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "usuario",
                    FieldType = SqlDbType.Int,
                    FieldLength = 11,
                    Value = Value.ForNumber(request.Usuario)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "rol",
                    FieldType = SqlDbType.Int,
                    FieldLength = 11,
                    Value = Value.ForNumber(request.Rol)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "bloqueados",
                    FieldType = SqlDbType.Int,
                    FieldLength = 11,
                    Value = Value.ForNumber((request.Bloqueados == true ? 1 : 0))
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "desde",
                    FieldType = SqlDbType.VarChar,
                    FieldLength = 50,
                    Value = Value.ForString(request.Desde)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "hasta",
                    FieldType = SqlDbType.VarChar,
                    FieldLength = 50,
                    Value = Value.ForString(request.Hasta)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "ejecutivo",
                    FieldType = SqlDbType.VarChar,
                    FieldLength = 50,
                    Value = Value.ForString(request.Ejecutivo)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "mtcid",
                    FieldType = SqlDbType.VarChar,
                    FieldLength = 50,
                    Value = Value.ForString(request.Mtcid)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "formaPago",
                    FieldType = SqlDbType.VarChar,
                    FieldLength = 50,
                    Value = Value.ForString(request.FormaPago)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "banco",
                    FieldType = SqlDbType.VarChar,
                    FieldLength = 50,
                    Value = Value.ForString(request.Banco)
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

        public DataRequestReply DetalleDepositos(LiberacionesRequestDetalleDepositos request)
        {

            DataRequestModel data = new DataRequestModel();
            DataRequestReply reply = new DataRequestReply();

            try
            {
                data.ProcedureName = "sp_getDetalleDeposito";
                data.TransactionDb = TransactionDb.Select;

                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "id",
                    FieldType = SqlDbType.Int,
                    FieldLength = 11,
                    Value = Value.ForNumber(request.IdDeposito)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "idUsuario",
                    FieldType = SqlDbType.Int,
                    FieldLength = 11,
                    Value = Value.ForNumber(request.IdUsuario)
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

        public DataRequestReply ContadorDepositosErroneos(LiberacionesRequestDetalleDepositos request)
        {
            DataRequestModel data = new DataRequestModel();
            DataRequestReply reply = new DataRequestReply();

            try
            {
                data.ProcedureName = "sp_cuentaDepositosErroneosClientes";
                data.TransactionDb = TransactionDb.Select;

                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "id",
                    FieldType = SqlDbType.Int,
                    FieldLength = 11,
                    Value = Value.ForNumber(request.IdDeposito)
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
