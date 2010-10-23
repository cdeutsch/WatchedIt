using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Infrastructure.Storage;
using Web.Reporting;

namespace Web.Infrastructure.Reporting
{
    public class ReportingSession : EFCFSession, IReporting
    {

        public ReportingSession() : base(new ReportingDB()) { }

    }
}
