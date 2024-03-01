using UnityEngine;
using Steamworks;

public class SteamGameContent : MonoBehaviour
{
    // Our GameID
    private CGameID m_GameID;

    // Did we get the stats from Steam?
    private bool m_bRequestedStats;
    private bool m_bStatsValid;

    // Should we store stats this frame?
    private bool m_bStoreStats;

    // Current Stat details
    private float m_flGameFeetTraveled;
    private float m_ulTickCountGameStart;
    private double m_flGameDurationSeconds;

    // Persisted Stat details
    private int m_nTotalGamesPlayed;
    private int m_nTotalNumWins;
    private int m_nTotalNumLosses;
    private float m_flTotalFeetTraveled;
    private float m_flMaxFeetTraveled;
    
    protected Callback<UserStatsReceived_t> m_UserStatsReceived;
    protected Callback<UserStatsStored_t> m_UserStatsStored;
    protected Callback<UserAchievementStored_t> m_UserAchievementStored;
    protected Callback<GameOverlayActivated_t> m_GameOverlayActivated; // L'overlay de steam est t'il activé ? -> Pause si oui
    
    void Start()
    {
        if(SteamManager.Initialized)
        {
            string name = SteamFriends.GetPersonaName();
            Debug.Log(name);

            if (!m_Achievements[0].m_bAchieved)
            {
                UnlockAchievement(m_Achievements[0]);
            }
        }
    }
    
    private void OnEnable() 
    {
        if (!SteamManager.Initialized) return; 
        
        m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
        m_GameID = new CGameID(SteamUtils.GetAppID());
        m_UserAchievementStored = Callback<UserAchievementStored_t>.Create(OnAchievementStored);

        // TODO - Mettre le jeu en pause si necessaire
        void OnGameOverlayActivated(GameOverlayActivated_t pCallback) {
            if(pCallback.m_bActive != 0) 
            {
                Debug.Log("Steam Overlay has been activated");
            }
            else 
            {
                Debug.Log("Steam Overlay has been closed");
            }
        }
    }
    
    #region Achievement
    private enum Achievement : int
    {
        ACH_WIN_ONE_GAME,
        ACH_WIN_10_GAMES,
        ACH_TRAVEL_FAR_ACCUM,
        ACH_TRAVEL_FAR_SINGLE,
        ACH_LAUNCH_GAME
    };
    
    private class Achievement_t {
        public Achievement m_eAchievementID;
        public string m_strName;
        public string m_strDescription;
        public bool m_bAchieved;

        /// <summary>
        /// Creates an Achievement. You must also mirror the data provided here in https://partner.steamgames.com/apps/achievements/yourappid
        /// </summary>
        /// <param name="achievement">The "API Name Progress Stat" used to uniquely identify the achievement.</param>
        /// <param name="name">The "Display Name" that will be shown to players in game and on the Steam Community.</param>
        /// <param name="desc">The "Description" that will be shown to players in game and on the Steam Community.</param>
        public Achievement_t(Achievement achievementID, string name, string desc) {
            m_eAchievementID = achievementID;
            m_strName = name;
            m_strDescription = desc;
            m_bAchieved = false;
        }
    }
    
    private Achievement_t[] m_Achievements = new Achievement_t[] {
        new Achievement_t(Achievement.ACH_LAUNCH_GAME, "You launch the game once !", "The entire Havocar Games team thanks you !"),
        new Achievement_t(Achievement.ACH_WIN_ONE_GAME, "Winner", "ACH_WIN_ONE_GAME"),
        new Achievement_t(Achievement.ACH_WIN_10_GAMES, "Champion", "ACH_WIN_10_GAMES"),
        new Achievement_t(Achievement.ACH_TRAVEL_FAR_ACCUM, "Une fleche dans le chaos", "Travel 10000 KM"),
        new Achievement_t(Achievement.ACH_TRAVEL_FAR_SINGLE, "Little road trip in chaos", "Travel 500 KM in one game")
    };
    
    /// <summary>
    /// Unlock an achievement
    /// </summary>
    /// <param name="achievement"></param>
    private void UnlockAchievement(Achievement_t achievement) {
        achievement.m_bAchieved = true;
        
        SteamUserStats.SetAchievement(achievement.m_eAchievementID.ToString());
        SteamUserStats.StoreStats();
    }
    
    /// <summary>
    /// An achievement was stored
    /// </summary>
    /// <param name="pCallback"></param>
    private void OnAchievementStored(UserAchievementStored_t pCallback) {
        // We may get callbacks for other games' stats arriving, ignore them
        if ((ulong)m_GameID == pCallback.m_nGameID) {
            if (0 == pCallback.m_nMaxProgress) {
                Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' unlocked!");
            }
            else {
                Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' progress callback, (" + pCallback.m_nCurProgress + "," + pCallback.m_nMaxProgress + ")");
            }
        }
    }
    #endregion
    
}
