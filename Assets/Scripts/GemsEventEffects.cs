using System.Threading.Tasks;
using UnityEngine;
public class GemsEventEffects : MonoBoardSubscriber<GemsEventEffects>
{
    [SerializeField]
    private Transform effectsParent;
    private IGemsCombiner UsedGemsCombiner => GemsCombiner.Instance;
    private SC_GameVariablesConfig GameVariables => SC_GameVariablesConfig.Instance();

    protected override async void SubscribeOnEvents()
    {
        base.SubscribeOnEvents();
        await Task.Yield();
        UsedGemsCombiner.DestroyMatchedGem.AddListener(OnGemDestroy);
    }

    protected override void UnSubscribeOnEvents()
    {
        base.UnSubscribeOnEvents();
        UsedGemsCombiner.DestroyMatchedGem.RemoveListener(OnGemDestroy);
    }

    private void OnGemDestroy(Gem _Gem)
    {
        SpawnParticleOfColorAt(_Gem.Transform.position, _Gem.Type);
    }

    private void SpawnParticleOfColorAt(Vector3 _Position, GlobalEnums.GemType _GemType)
    {
        var prefab = GameVariables.EffectPrefabByType(_GemType);
        var effect = Instantiate(prefab, effectsParent);
        effect.transform.position = _Position;
        var effectLength = effect.GetComponent<ParticleSystem>().totalTime;
        Destroy(effect, effectLength);
    }
}
