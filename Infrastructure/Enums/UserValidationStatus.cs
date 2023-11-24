namespace Infrastructure.Enums
{
    public enum UserValidationStatus
    {
        Valid,
        NotValid_Name_Is_NullOrEmpty,
        NotValid_NameLength_Too_Short,
        NotValid_Name_Contains_Invalid_Characters,
        NotValid_UserName_Is_NullOrEmpty,
        NotValid_UserNameLength_Too_Short,
        NotValid_Password_Does_Not_Meet_Requirements,

    }
}
