using System.Collections.Generic;
using System.Linq;
using Unity1week202504.Data;
using Unity1week202504.Data.Ending;
using Unity1week202504.InGame.Memories;
using ZLinq;

namespace Unity1week202504.Ending
{
    public class EndingCalculator
    {
        private readonly EndingMasterDataSource _masterDataSource;

        public EndingCalculator(EndingMasterDataSource masterDataSource)
        {
            _masterDataSource = masterDataSource;
        }
        
        public EndingMasterData Calculate(IReadOnlyCollection<MemoryId> memoryIds)
        {
            // 候補
            var matchEndingMasterData = _masterDataSource.All.AsValueEnumerable()
                .Where(data =>
                {
                    return data.ConditionMemoryIds
                        .Where(x => !x.IsEmpty) // EmptyはAny扱い
                        .All(memoryIds.Contains); // すべての条件を満たす
                });
            
            // すべての条件を満たすものがあればそれを返す
            var result = matchEndingMasterData.OrderByDescending(x => x.Priority).First();
            return result;
        }
    }
}