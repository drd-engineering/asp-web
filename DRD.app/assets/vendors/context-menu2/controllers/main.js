'use strict';

/**
 * @ngdoc function
 * @name contextMenuApp.controller:MainCtrl
 * @description
 * # MainCtrl
 * Controller of the contextMenuApp
 */
angular.module('contextMenuApp')
    .controller('MainCtrl', function ($scope) {

        //temp for table/preview
        $scope.items = [];
        for (var i = 1; i <= 20; i++) {
            $scope.items.push({name: 'Name ', address: 'Address '})
        }
        //Menu Items Array
        $scope.menus = [
            {label: 'View', action: 'callView', active: true},
            {label: 'Delete', action: 'deleteItem', active: true},
            {label: 'Send', action: 'sendItem', active: false},
            {label: 'Share', action: '', active: true},
            {label: 'Active', action: 'deactivate', active: false}
        ];

        //Sample Button Dropdown
        $scope.buttonMenu = [
            {label: 'View Large', action: 'callView', active: true},
            {label: 'Delete this item', action: 'deleteItem', active: true}
        ];

        $scope.deleteItem = function (arg) {
            console.warn('deleted ...')
        };

        $scope.callView = function (arg) {
            console.info('View Call, another method')
        };

    });

