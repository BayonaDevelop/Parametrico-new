﻿using Com.Coppel.SDPC.Application.Models.Enums;
using Com.Coppel.SDPC.Application.Models.Persistence;
using Com.Coppel.SDPC.Application.Models.Reports;

namespace Com.Coppel.SDPC.Application.Commons.Files;

public interface IServicePdfFactorSaturacion
{
	string CreateFile(FileContentVM request, string logoPath);
}
