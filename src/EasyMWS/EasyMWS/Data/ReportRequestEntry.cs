﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MountainWarehouse.EasyMWS.CallbackLogic;
using MountainWarehouse.EasyMWS.Enums;
using MountainWarehouse.EasyMWS.Model;
using Newtonsoft.Json;

namespace MountainWarehouse.EasyMWS.Data
{
	public class ReportRequestEntry : IRestrictionableInvocationEntry
    {
		private string _regionAndType;
		[NotMapped]
		public string RegionAndTypeComputed
		{
			// this field is populated based on ReportRequestData which, once set in the ctor, should never change again for the same entity.
			get { return _regionAndType = _regionAndType ?? $"(ReportType:{ReportType},Region:{AmazonRegion.ToString()})"; }
		}

		[Key]
		public int Id { get; set; }
		public bool IsLocked { get; set; }
		public int ReportRequestRetryCount { get; set; }
		public int ReportDownloadRetryCount { get; set; }
		public int InvokeCallbackRetryCount { get; set; }
		public int ReportProcessRetryCount { get; set; }
		public DateTime LastAmazonRequestDate { get; set; }
        public string LastAmazonReportProcessingStatus { get; set; }
		public DateTime DateCreated { get; set; }

		#region Callback data necessary to invoke a method with it's argument values.

        public string TargetHandlerId { get; set; }
        public string TargetHandlerArgs { get; set; }
        public string InstanceId { get; set; }

        #endregion

        #region Data necessary to request a report from amazon.

        public AmazonRegion AmazonRegion { get; set; }
		public string ReportType { get; set; }
		public string MerchantId { get; set; }
		public ContentUpdateFrequency ContentUpdateFrequency { get; set; }
		public string ReportRequestData { get; set; }


		public virtual ReportRequestDetails Details { get; set; }

		#endregion

		#region Additional data generated by amazon in the process of fetching reports

		/// <summary>The ID that Amazon has given us for this requested report</summary>
		public string RequestReportId { get; set; }

		/// <summary>The ID that Amazon gives us when the report has been generated (required to download the report)</summary>
		public string GeneratedReportId { get; set; }

		#endregion



		public ReportRequestEntry()
		{
		}

		public ReportRequestEntry(string reportRequestData)
		{
			if(string.IsNullOrEmpty(reportRequestData))
				throw new ArgumentException("ReportRequestEntry data not provided, but it is required");

			ReportRequestData = reportRequestData;
            LastAmazonReportProcessingStatus = null;
        }
	}

	internal static class ReportRequestCallbackExtensions
	{
		internal static ReportRequestPropertiesContainer GetPropertiesContainer(this ReportRequestEntry source)
		{
			return JsonConvert.DeserializeObject<ReportRequestPropertiesContainer>(source.ReportRequestData);
		}
	}
}
