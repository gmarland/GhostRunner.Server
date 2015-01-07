using GhostRunner.Server.DAL.Interface;
using GhostRunner.Server.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostRunner.Server.DAL
{
    public class SequenceDataAccess : ISequenceDataAccess
    {
        protected IContext _context;

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SequenceDataAccess(IContext context)
        {
            _context = context;
        }

        public Sequence Get(int sequenceId)
        {
            try
            {
                return _context.Sequences.SingleOrDefault(s => s.ID == sequenceId);
            }
            catch (Exception ex)
            {
                _log.Error("Get(" + sequenceId + "): Error retrieving sequence", ex);

                return null;
            }
        }
    }
}
