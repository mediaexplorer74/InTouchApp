// Decompiled with JetBrains decompiler
// Type: Splunk.Mi.Utilities.SplunkTransactionManager
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using BugSense.Core;
using BugSense.Core.Interfaces;
using BugSense.Core.Model;
using BugSense.Device.Specific;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Splunk.Mi.Utilities
{
  internal class SplunkTransactionManager
  {
    private IBugSenseFileClient FileRepo { get; set; }

    private IDeviceUtil DeviceInfo { get; set; }

    private IRequestJsonSerializer JsonSerializer { get; set; }

    private List<SplunkTransaction> Transactions { get; set; }

    public SplunkTransactionManager(
      IBugSenseFileClient fileRepo,
      IDeviceUtil deviceUtil,
      IRequestJsonSerializer jsonSerializer)
    {
      this.Transactions = new List<SplunkTransaction>();
      this.FileRepo = fileRepo;
      this.DeviceInfo = deviceUtil;
      this.JsonSerializer = jsonSerializer;
    }

    public Task<TransactionStartResult> TransactionStart(string transactionId)
    {
      return Task.Run<TransactionStartResult>((Func<Task<TransactionStartResult>>) (async () =>
      {
        TransactionStartResult result = new TransactionStartResult()
        {
          Status = TransactionStatus.Failed,
          TransactionId = transactionId
        };
        bool isTransactionExist = this.Transactions.Any<SplunkTransaction>((Func<SplunkTransaction, bool>) (p => p.TransactionId.Equals(transactionId, StringComparison.OrdinalIgnoreCase)));
        if (!isTransactionExist)
        {
          TrStart transactionStart = TrStart.GetInstance(transactionId, this.DeviceInfo.GetAppEnvironment(), this.DeviceInfo.GetBugSensePerformance());
          this.Transactions.Add(new SplunkTransaction()
          {
            TransactionId = transactionId,
            TransactionStart = transactionStart
          });
          SerializeResult serializedResult = this.JsonSerializer.SerializeTransaction<TrStart>(transactionStart);
          BugSenseLogResult logResult = await this.FileRepo.SaveAsync(CommonHelper.NewFileNamePath(FileNameType.TransactionStart), serializedResult.EncodedJson);
          logResult.ClientRequest = serializedResult.DecodedJson;
          if (logResult.ResultState == BugSenseResultState.OK)
          {
            result.Status = TransactionStatus.SuccessfullyStarted;
            result.TransactionStart = transactionStart;
          }
          else
            result.Status = TransactionStatus.Failed;
        }
        else
        {
          SplunkTransaction splunkTransaction = this.Transactions.FirstOrDefault<SplunkTransaction>((Func<SplunkTransaction, bool>) (p => p.TransactionId.Equals(transactionId, StringComparison.OrdinalIgnoreCase)));
          if (splunkTransaction != null)
          {
            result.Status = TransactionStatus.Exists;
            result.TransactionStart = splunkTransaction.TransactionStart;
          }
          else
            result.Status = TransactionStatus.Failed;
        }
        return result;
      }));
    }

    public Task<TransactionStopResult> TransactionStop(string transactionId)
    {
      return this.TransactionStop(transactionId, (string) null, TransactionCompletedStatus.SUCCESS, TransactionStatus.UserSuccessfullyStopped);
    }

    public Task<TransactionStopResult> TransactionCancel(string transactionId, string reason)
    {
      return this.TransactionStop(transactionId, reason, TransactionCompletedStatus.CANCEL, TransactionStatus.UserCancelled);
    }

    public void StopAll(string errorHash)
    {
      List<string> stringList = new List<string>();
      foreach (SplunkTransaction transaction in this.Transactions)
      {
        TrStop instance = TrStop.GetInstance(transaction.TransactionId, this.DeviceInfo.GetAppEnvironment(), transaction.Elapsed, errorHash, TransactionCompletedStatus.FAIL.ToString());
        transaction.TransactionStop = instance;
        SerializeResult serializeResult = this.JsonSerializer.SerializeTransaction<TrStop>(instance);
        if (this.FileRepo.Save(CommonHelper.NewFileNamePath(FileNameType.TransactionStop), serializeResult.EncodedJson).ResultState == BugSenseResultState.OK)
          stringList.Add(transaction.TransactionId);
      }
      foreach (string str in stringList)
      {
        string transactionid = str;
        SplunkTransaction splunkTransaction = this.Transactions.FirstOrDefault<SplunkTransaction>((Func<SplunkTransaction, bool>) (p => p.TransactionId.Equals(transactionid, StringComparison.OrdinalIgnoreCase)));
        if (splunkTransaction != null)
          this.Transactions.Remove(splunkTransaction);
      }
    }

    public Task<TransactionStopResult> TransactionStop(
      string transactionId,
      string reason,
      TransactionCompletedStatus completedStatus,
      TransactionStatus userStatus)
    {
      return Task.Run<TransactionStopResult>((Func<Task<TransactionStopResult>>) (async () =>
      {
        TransactionStopResult result = new TransactionStopResult()
        {
          Status = TransactionStatus.Failed
        };
        SplunkTransaction transaction = this.Transactions.FirstOrDefault<SplunkTransaction>((Func<SplunkTransaction, bool>) (p => p.TransactionId.Equals(transactionId, StringComparison.OrdinalIgnoreCase)));
        if (transaction != null)
        {
          TrStop transactionStop = TrStop.GetInstance(transactionId, this.DeviceInfo.GetAppEnvironment(), transaction.Elapsed, reason, completedStatus.ToString());
          transaction.TransactionStop = transactionStop;
          SerializeResult serializedResult = this.JsonSerializer.SerializeTransaction<TrStop>(transactionStop);
          BugSenseLogResult logResult = await this.FileRepo.SaveAsync(CommonHelper.NewFileNamePath(FileNameType.TransactionStop), serializedResult.EncodedJson);
          logResult.ClientRequest = serializedResult.DecodedJson;
          if (logResult.ResultState == BugSenseResultState.OK)
          {
            result.Status = userStatus;
            result.TransactionStop = transactionStop;
            this.Transactions.Remove(transaction);
          }
          else
            result.Status = TransactionStatus.Failed;
        }
        else
          result.Status = TransactionStatus.NotFound;
        return result;
      }));
    }
  }
}
