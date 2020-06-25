var status = {
    OK = "OK",
    ERROR_WORKFLOW_START_NODE_NOT_FOUND = "ERROR_WORKFLOW_START_NODE_NOT_FOUND",
    ERROR_ROTATION_ALREADY_STARTED = "ERROR_ROTATION_ALREADY_STARTED",
    ADMINISTRATOR_EXCEED_LIMIT = "ADMINISTRATOR_EXCEED_LIMIT",
    MEMBER_EXCEED_LIMIT = "MEMBER_EXCEED_LIMIT",
    ROTATION_STARTED_EXCEED_LIMIT = "ROTATION_STARTED_EXCEED_LIMIT",
    NO_ACTIVE_PLAN = "NO_ACTIVE_PLAN",
    NOT_AUTHORIZED = "NOT_AUTHORIZED",
    EXPIRED = "EXPIRED",
    STORAGE_EXCEED_LIMIT = "STORAGE_EXCEED_LIMIT"
}

var message = {
    OK = "OK",
    ERROR_WORKFLOW_START_NODE_NOT_FOUND = "ERROR_WORKFLOW_START_NODE_NOT_FOUND",
    ERROR_ROTATION_ALREADY_STARTED = "ERROR_ROTATION_ALREADY_STARTED",
    ADMINISTRATOR_EXCEED_LIMIT = "ADMINISTRATOR_EXCEED_LIMIT",
    MEMBER_EXCEED_LIMIT = "MEMBER_EXCEED_LIMIT",
    ROTATION_STARTED_EXCEED_LIMIT = "ROTATION_STARTED_EXCEED_LIMIT",
    NO_ACTIVE_PLAN = "NO_ACTIVE_PLAN",
    NOT_AUTHORIZED = "NOT_AUTHORIZED",
    EXPIRED = "EXPIRED",
    STORAGE_EXCEED_LIMIT = "STORAGE_EXCEED_LIMIT"
}



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

    