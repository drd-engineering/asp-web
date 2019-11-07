'use strict';

/**
 * @ngdoc overview
 * @name contextMenuApp
 * @description
 * # contextMenuApp
 *
 * Main module of the application.
 */
angular
  .module('contextMenuApp', [
    'ngAnimate',
    'ngCookies',
    'ngResource',
    'ngRoute',
    'ngSanitize',
    'ngTouch'
  ])
  .config(function ($routeProvider) {
    $routeProvider
      .when('/', {
        templateUrl: 'views/main.html',
        controller: 'MainCtrl'
      })
      .otherwise({
        redirectTo: '/'
      });
  });
