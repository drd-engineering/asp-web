const InivitationStatus = {
    Connected = 0,
    Pending = 1,
    Inactive = 2
}
s


const MemberRole = {
    Not_Member = 0,
    Member = 1,
    Administrator = 2,
    Owner = 3
}
const RotationStatus = {
    Open = 0,
    In_Progress = 1,
    Pending = 2,
    Signed = 3,
    Revision = 5,
    Altered = 6,
    Completed = 90,
    Declined = 98,
    Canceled = 99,
    Waiting_For_Response = 10,
    Accepted = 11,
    Expired = 97
}

const AccessType = {
    //restricted, cannot access at all
    noAccess = 0,
    //read only access
    readOnly = 1,
    //can access page
    responsible = 2,
    //fully access granted if there are multiple feature access in a pages
    fullAccess = 3
}

    