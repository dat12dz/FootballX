using UnityEngine;

namespace Assets.Script.Setting
{
    internal class TerrianSettingApplier : MonoBehaviour
    {
        Terrain t;
    SettingDAO setting = SettingDAO.Instance;
        private void Start()
        {
            t = GetComponent<Terrain>();
            t.drawTreesAndFoliage = setting.graphicSetting.ChosenPreset != 0;
        }
    }
}
