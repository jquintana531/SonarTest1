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
    public class UsuarioManager
    {
        private DBService _client;
        public UsuarioManager(DBData.DBDataClient client)
        {
            this._client = new DBService(client);

        }

        public DataRequestReply LoginManager(Credential credenciales)
        {
            DataRequestModel data = new DataRequestModel();
            DataRequestReply reply = new DataRequestReply();

            try
            {
                data.ProcedureName = "AUTH_validaUsuarioBackOffice";
                data.TransactionDb = TransactionDb.Select;

                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "idDispositivo",
                    FieldType = SqlDbType.VarChar,
                    Value = Value.ForString(credenciales.IdentificadorDispositivo),
                    FieldLength = 50,
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "usuario",
                    FieldType = SqlDbType.VarChar,
                    Value = Value.ForString(credenciales.Usuario),
                    FieldLength = 50,
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "clave",
                    FieldType = SqlDbType.VarChar,
                    Value = Value.ForString(credenciales.Password),
                    FieldLength = 50,
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "ip",
                    FieldType = SqlDbType.VarChar,
                    Value = Value.ForString(credenciales.Ip),
                    FieldLength = 50,
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


        public DataRequestReply CambioClaveManager(NewPasswordModel credenciales)
        {
            DataRequestModel data = new DataRequestModel();
            DataRequestReply reply = new DataRequestReply();

            try
            {
                data.ProcedureName = "sp_setNuevaClaveUsuario";
                data.TransactionDb = TransactionDb.Select;

                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "id",
                    FieldType = SqlDbType.VarChar,
                    Value = Value.ForString(credenciales.Usuario.ToString()),
                    FieldLength = 50,
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "claveNueva",
                    FieldType = SqlDbType.VarChar,
                    Value = Value.ForString(credenciales.PassNueva),
                    FieldLength = 50,
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "claveActual",
                    FieldType = SqlDbType.VarChar,
                    Value = Value.ForString(credenciales.PassAntigua),
                    FieldLength = 50,
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "idUsuario",
                    FieldType = SqlDbType.VarChar,
                    Value = Value.ForString(credenciales.Usuario.ToString()),
                    FieldLength = 15,
                });
                data.InputParameter.Add(new InputParameter()
                {
                    FieldName = "ip",
                    FieldType = SqlDbType.VarChar,
                    Value = Value.ForString(credenciales.Ip),
                    FieldLength = 15,
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
    }
}
