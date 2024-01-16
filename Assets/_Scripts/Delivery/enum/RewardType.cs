[System.Serializable]
enum RewardType
{
    ObjectDelivery,
    IncrementValue
}


[System.Serializable]
public enum SpawnDeliveryState
{
    IsOrNotDelivered,
    Delivered
}


[System.Serializable]
public enum AntennaState
{
    IsInactive,
    IsBeingCaptured,
    IsBeingLeaved,
    AntennaIsActivated
}
