var dataArray1 = [
    {
        "city": "Beijing",
        "value": 132
    },
    {
        "city": "Shanghai",
        "value": 422
    },
    {
        "city": "Chengdu",
        "value": 232
    },
    {
        "city": "Wuhan",
        "value": 765
    },
    {
        "city": "Tianjin",
        "value": 876
    },
    {
        "city": "Guangzhou",
        "value": 453
    },
    {
        "city": "Hongkong",
        "value": 125
    }
];

var settings1 = {
    "dataArray": dataArray1,
    "itemName": "city", 
    "valueName": "value",
    "callable": function (items) {
        console.dir(items)
    }
};

var adminsManage = $("#manage-admin-content").transfer(settings1);

var selecteAdminsd = adminsManage.getSelectedItems();