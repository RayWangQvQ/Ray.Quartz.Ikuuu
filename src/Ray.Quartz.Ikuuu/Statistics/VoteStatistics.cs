using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Ray.Quartz.Ikuuu.Models;

namespace Ray.Quartz.Ikuuu.Statistics
{
    public class VoteStatistics
    {
        public CreditHistory PreCreditHistory { get; set; }

        public CreditHistory AfterCreditHistory { get; set; }

        public List<string> VotePosts { get; set; }=new List<string>();

        public void LogInfo(ILogger logger)
        {
            logger.LogInformation("=====================统计======================");

            logger.LogInformation("任务前状态：");
            PreCreditHistory?.LogInfo(logger);

            logger.LogInformation("任务后状态：");
            AfterCreditHistory?.LogInfo(logger);

            logger.LogInformation("共成功访问了{num}个空间：",VotePosts.Count);
            logger.LogInformation(VotePosts.JoinAsString(","));
        }
    }
}
