using Core;
using Gems;
using TMPro;
using UnityEngine;
namespace Common
{
    public class GameScoring : MonoBoardSubscriber<GameScoring>
    {
        [SerializeField]
        private TextMeshProUGUI scoreText;

        private int score;
    
        private IGemsCombiner UsedGemsCombiner => GemsCombiner.Instance;
        private SC_GameVariablesConfig GameVariables => SC_GameVariablesConfig.Instance();

        private void Start()
        {
            UpdateScoreText();
        }

        protected override void SubscribeOnEvents()
        {
            base.SubscribeOnEvents();
            UsedGemsCombiner.DestroyMatchedGem.AddListener(OnGemDestroy);
            UsedGemsCombiner.SpawnedBombInsteadOfGem.AddListener(OnGemDestroy);
        }

        protected override void UnSubscribeOnEvents()
        {
            base.UnSubscribeOnEvents();
            UsedGemsCombiner.DestroyMatchedGem.RemoveListener(OnGemDestroy);
            UsedGemsCombiner.SpawnedBombInsteadOfGem.RemoveListener(OnGemDestroy);
        }

        private void OnGemDestroy(Gem _Gem)
        {
            score += GameVariables.scoreByGem;
            UpdateScoreText();
        }

        private void UpdateScoreText()
        {
            scoreText.text = score.ToString();
        }
    }
}
