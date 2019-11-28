

$(function () {

    var data = {
        menu: [{
            name: 'Dashboard',
            icon: '', 
            link: '0',
            sub: null
        }, {
            name: 'England',
            link: '1',
            sub: [{
                name: 'Arsenal',
                link: '0-0',
                sub: null
            }, {
                name: 'Liverpool',
                link: '0-1',
                sub: null
            }, {
                name: 'Manchester United',
                link: '0-2',
                sub: null
            }]
        }, {
            name: 'Spain',
            link: '2',
            sub: [{
                name: 'Barcelona',
                link: '2-0',
                sub: null
            }, {
                name: 'Real Madrid',
                link: '2-1',
                sub: null
            }]
        }, {
            name: 'Germany',
            link: '3',
            sub: [{
                name: 'Bayern Munich',
                link: '3-1',
                sub: null
            }, {
                name: 'Borrusia Dortmund',
                link: '3-2',
                sub: null
            }]
        }]
    };
    var getMenuItem = function (itemData) {
        var item = $("<li>")
            .append(
        $("<a>", {
            href: '#' + itemData.link,
            html: itemData.name
        }));
        if (itemData.sub) {
            var subList = $("<ul>");
            $.each(itemData.sub, function () {
                subList.append(getMenuItem(this));
            });
            item.append(subList);
        }
        return item;
    };
    
    var $menu = $("#menu");
    $.each(data.menu, function () {
        $menu.append(
            getMenuItem(this)
        );
    });
    $menu.menu();
});