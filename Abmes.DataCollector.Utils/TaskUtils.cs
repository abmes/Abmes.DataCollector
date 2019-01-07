using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Utils
{
    public static class TaskUtils
    {
        public static async Task TryTaskAsync(Func<int, Task> getTask, Func<Exception, bool> isRetryableError, int tryCount, TimeSpan retryDelay)
        {
            try
            {
                await getTask(1);
            }
            catch (Exception e)
            {
                if (isRetryableError(e))
                {
                    var retryCount = tryCount - 1;
                    int retryCounter = 0;
                    while (retryCounter < retryCount)
                    {
                        try
                        {
                            await Task.Delay(retryDelay);
                            await getTask(retryCounter + 2);
                            break;
                        }
                        catch (Exception e2)
                        {
                            if (!isRetryableError(e2))
                            {
                                throw e2;
                            }
                        }

                        retryCounter++;
                    }

                    if (retryCounter == retryCount)
                    {
                        throw;
                    }
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
