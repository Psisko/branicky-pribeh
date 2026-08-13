public struct DamageResponseData {

    public bool isSolid;
    public bool wasParried;

    public DamageResponseData(bool isSolid, bool wasParried) {
        this.wasParried = wasParried;
        this.isSolid = isSolid;
    }
}