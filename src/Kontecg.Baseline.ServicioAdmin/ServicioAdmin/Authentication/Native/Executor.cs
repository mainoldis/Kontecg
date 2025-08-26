using System.Data;

namespace Kontecg.Baseline.ServicioAdmin.Authentication.Native
{
    internal class Executor
    {
        private readonly IDbCommand _command;

        public Executor(IDbCommand command)
        {
            _command = command;
        }

        /// <summary>
        ///     executes the query as a datareader
        /// </summary>
        /// <returns></returns>
        public IDataReader Reader()
        {
            return Prepare().ExecuteReader();
        }

        /// <summary>
        ///     Executes the command, returning the first column of the first result as a scalar value
        /// </summary>
        /// <returns></returns>
        public object Scalar()
        {
            object result = Prepare().ExecuteScalar();
            return result;
        }

        /// <summary>
        ///     Executes the command as a SQL statement, returning the number of rows affected
        /// </summary>
        public int NonQuery()
        {
            return Prepare().ExecuteNonQuery();
        }

        private IDbCommand Prepare()
        {
            Logger.Log(_command.CommandText);
            foreach (IDbDataParameter p in _command.Parameters)
            {
                Logger.Log(string.Format("{0} = {1}", p.ParameterName, p.Value));
            }

            if (_command.Connection.State == ConnectionState.Closed)
            {
                _command.Connection.Open();
            }

            return _command;
        }
    }
}
