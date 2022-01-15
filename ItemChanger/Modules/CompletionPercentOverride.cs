using ItemChanger.Extensions;
using ItemChanger.Internal;
using ItemChanger.Tags;
using Newtonsoft.Json;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Configurable module that overrides the completion percentage.
    /// Items and Placements will be weighted according to their CompletionWeightTag.
    /// Transition weights are set in the module.
    /// By default, finding an item increases the total by 1/(#items at placement),
    /// and finding a transition X->Y, Y->X pair increases the total by 1.
    /// </summary>
    public class CompletionPercentOverride : Module
    {
        /// <summary>
        /// If this is true, items will be scaled so that placements have total weight equal to their tag (or 1).
        /// </summary>
        public bool NormalizePlacementCounts { get; set; } = true;

        public HashSet<Transition> FoundTransitions = new();
        [JsonConverter(typeof(Transition.TransitionDictConverter<float>))]
        public Dictionary<Transition, float> TransitionWeights = new();

        public void SetTransitionWeight(Transition t, float weight)
        {
            TransitionWeights[t] = weight;
        }

        /// <summary>
        /// If this is true, finding transition X -> Y will mark both X and Y as found.
        /// Otherwise, just X will be marked.
        /// </summary>
        public bool CoupledTransitions { get; set; } = true;

        public override void Initialize()
        {
            // Weight start by 0, if not overridden.
            if (Ref.Settings.Placements.TryGetValue(LocationNames.Start, out AbstractPlacement start))
            {
                if (!start.HasTag<CompletionWeightTag>())
                {
                    start.AddTag<CompletionWeightTag>().Weight = 0;
                }
            }

            Events.OnTransitionOverride += MarkChangedTransition;
            AbstractItem.AfterGiveGlobal += InvalidateCacheOnObtainItem;
            On.PlayerData.CountGameCompletion += OverrideCompletionPercentage;
        }

        public override void Unload()
        {
            Events.OnTransitionOverride -= MarkChangedTransition;
            AbstractItem.AfterGiveGlobal -= InvalidateCacheOnObtainItem;
        }

        public (float, float) GetItemFraction()
        {
            float total = 0;
            float obtained = 0;
            foreach (AbstractPlacement pmt in Ref.Settings.GetPlacements())
            {
                if (pmt.Items.Count == 0) continue;

                float placementWeight = pmt.GetTag<CompletionWeightTag>()?.Weight ?? 1f;
                if (placementWeight == 0) continue;

                float totalItemWeight = 0;
                float obtainedItemWeight = 0;
                foreach (AbstractItem item in pmt.Items)
                {
                    float itemWeight = item.GetTag<CompletionWeightTag>()?.Weight ?? 1f;
                    totalItemWeight += itemWeight;
                    if (item.WasEverObtained())
                    {
                        obtainedItemWeight += itemWeight;
                    }
                }
                if (totalItemWeight == 0) continue;

                if (NormalizePlacementCounts)
                {
                    obtainedItemWeight /= totalItemWeight;
                    totalItemWeight = 1;
                }
                total += totalItemWeight * placementWeight;
                obtained += obtainedItemWeight * placementWeight;
            }

            return (obtained, total);
        }

        public (float, float) GetTransitionFraction()
        {
            float obtained = 0;
            float total = 0;
            foreach (Transition t in Ref.Settings.TransitionOverrides.Keys)
            {
                total += TransitionWeights.GetOrDefault(t, 1);
                if (FoundTransitions.Contains(t))
                {
                    obtained += TransitionWeights.GetOrDefault(t, 1);
                }
            }

            if (CoupledTransitions)
            {
                obtained *= 0.5f;
                total *= 0.5f;
            }

            return (obtained, total);
        }

        public float ComputeCompletion(bool invalidateCache = false)
        {
            if (invalidateCache)
            {
                CacheValid = false;
            }
            else if (CacheValid)
            {
                return CachedCompletionValue;
            }

            (float itemNumerator, float itemDenominator) = GetItemFraction();
            (float transitionNumerator, float transitionDenominator) = GetTransitionFraction();
            if (itemDenominator == 0)
            {
                CacheValid = true;
                return CachedCompletionValue = transitionNumerator / transitionDenominator;
            }
            else if (transitionDenominator == 0)
            {
                CacheValid = true;
                return CachedCompletionValue = itemNumerator / itemDenominator;
            }
            else
            {
                CacheValid = true;
                return CachedCompletionValue = (itemNumerator + transitionNumerator) / (itemDenominator + transitionDenominator);
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        private bool CacheValid = false;

        [Newtonsoft.Json.JsonIgnore]
        private float CachedCompletionValue;

        private void InvalidateCacheOnObtainItem(ReadOnlyGiveEventArgs args)
        {
            if (args.OriginalState == ObtainState.Unobtained) CacheValid = false;
        }

        private void MarkChangedTransition(Transition source, Transition origTarget, ITransition newTarget)
        {
            if (FoundTransitions.Add(source)) CacheValid = false;
            if (CoupledTransitions)
            {
                if (FoundTransitions.Add(new(newTarget.SceneName, newTarget.GateName))) CacheValid = false;
            }
        }
        private void OverrideCompletionPercentage(On.PlayerData.orig_CountGameCompletion orig, PlayerData self)
        {
            float rawCompletion = ComputeCompletion() * 100;
            self.SetFloat(nameof(PlayerData.completionPercentage), Mathf.Floor(rawCompletion));
        }
    }
}
