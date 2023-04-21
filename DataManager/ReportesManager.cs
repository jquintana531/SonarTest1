using Google.Protobuf.WellKnownTypes;
using MTC.WebApp.BackOffice.Helpers;
using MTC.WebApp.BackOffice.Models;
using MTCenter.GRPC.GDBBO.DBProtos;
using System;

namespace MTC.WebApp.BackOffice.DataManager
{
    public class ReportesManager
    {
        private DBService _client;

        public ReportesManager(DBData.DBDataClient client)
        {
            this._client = new DBService(client);
        }

        #region Reporte Conciliaciones

        public DataRequestReply ReporteConciliaciones(ReporteConciliacionesRequest request)
        {
            DataRequestModel data = new DataRequestModel();
            DataRequestReply reply = new DataRequestReply();

            try
            {
                data.ProcedureName = "sp_reporteConciliaciones";
                data.TransactionDb = TransactionDb.Select;

                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "desde",
                    FieldType = SqlDbType.DateTime,
                    FieldLength = 30,
                    Value = Value.ForString(request.desde.ToString())
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "hasta",
                    FieldType = SqlDbType.DateTime,
                    FieldLength = 30,
                    Value = Value.ForString(request.hasta.ToString())
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "idCadena",
                    FieldType = SqlDbType.Int,
                    FieldLength = 11,
                    Value = Value.ForNumber(request.idCadena)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "idTienda",
                    FieldType = SqlDbType.Int,
                    FieldLength = 30,
                    Value = Value.ForNumber(request.idTienda)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "clasificacion",
                    FieldType = SqlDbType.Int,
                    FieldLength = 30,
                    Value = Value.ForNumber(request.clasificacion)
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "referencia",
                    FieldType = SqlDbType.VarChar,
                    FieldLength = int.MaxValue,
                    Value = Value.ForString(request.referencia)
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
