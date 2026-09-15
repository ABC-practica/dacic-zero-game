using UnityEngine;

namespace DacicZero.Data {
    [CreateAssetMenu(fileName = "NewMissionData", menuName = "DacicZero/Mission Data", order = 1)]
    public class MissionDataSO : ScriptableObject {
        
        [Header("Mission Identity")]
        [Tooltip("unique identifier for this mission")]
        [SerializeField] private string _missionId;

        [Tooltip("title displayed in mission selector")]
        [SerializeField] private string _missionTitle;

        [Tooltip("description of mission objectives")]
        [TextArea(3, 5)]
        [SerializeField] private string _missionDescription;

        [Tooltip("preview image for the mission overlay")]
        [SerializeField] private Sprite _missionImage;

        [Header("Scene Routing")]
        [Tooltip("scene name to load for this mission")]
        [SerializeField] private string _sceneName;

        [Tooltip("-1 uses scene name instead of build index")]
        [Min(-1)] // prevents invalid indices
        [SerializeField] private int _sceneBuildIndex = -1;

        [Header("Mission Status")]
        [Tooltip("do not save runtime player progress in ScriptableObjects")]
        [SerializeField] private bool _isCleared;

        public string MissionId => _missionId;
        public string MissionTitle => _missionTitle;
        public string MissionDescription => _missionDescription;
        public Sprite MissionImage => _missionImage;
        public string SceneName => _sceneName;
        public int SceneBuildIndex => _sceneBuildIndex;

        public bool IsCleared {
            get => _isCleared;
            set => _isCleared = value;
        }
    }
}