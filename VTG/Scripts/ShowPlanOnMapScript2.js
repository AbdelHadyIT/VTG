$(document).ready(console.log(NearsetNeighborTSP([
                [0, 35, 70, 105, 165],
                [35, 0, 45, 20, 80],
                [70, 45, 0, 30, 75],
                [105, 20, 30, 0, 60],
                [165, 80, 75, 60, 0]])));

var map;
var IMPERIAL;
var METRIC;
var markers=[];
var originsSameDestinations = [];
var path = [];
var result;
function initMap(){
    var directionsDisplay ;
    var directionsService = new google.maps.DirectionsService;
    var infoWindow = new google.maps.InfoWindow;
    var latitude = 34.80208544142896; // YOUR LATITUDE VALUE
    var longitude = 38.996811152237456; // YOUR LONGITUDE VALUE

    var myLatLng = { lat: latitude, lng: longitude };

    map = new google.maps.Map(document.getElementById('map'), {
        center: myLatLng,
        zoom: 7,
        disableDoubleClickZoom: true, // disable the default map zoom on double click
    });
    directionsDisplay = new google.maps.DirectionsRenderer({ map: map, suppressMarkers: false, suppressInfoWindows : true});
        
    initialize();
    setTimeout(function () {
        var stepDisplay = new google.maps.InfoWindow;
        set.forEach(function (item) {
            var infowincontent = document.createElement('div');
            var strong = document.createElement('strong');
            strong.textContent = item.name;
            infowincontent.appendChild(strong);
            infowincontent.appendChild(document.createElement('br'));
            var address = document.createElement('strong');
            address.textContent = 'address: ' + item.address;
            infowincontent.appendChild(address);
            infowincontent.appendChild(document.createElement('br'));
            var type = document.createElement('strong');
            infowincontent.appendChild(type);
            type.textContent = 'type: ' + item.type;
            infowincontent.appendChild(document.createElement('br'));
            var geoType = document.createElement('strong');
            geoType.textContent = 'Geographical type: ' + item.geoType;
            infowincontent.appendChild(geoType);
            var icon = { label: result[item.rank].toString() };
            var point = new google.maps.LatLng(
                        parseFloat(item.lat),
                        parseFloat(item.lng));
            var marker = new google.maps.Marker({
                map: map,
                position: point,
                label: icon.label
            });
            marker.addListener("click", function (event) {
                infoWindow.setContent(infowincontent);
                infoWindow.open(map, marker);
                calculateAndDisplayRoute(directionsService, directionsDisplay, first, marker.getPosition().lat() + ', ' + marker.getPosition().lng(), markers, stepDisplay, map);
            });
        });
    },4000);
    IMPERIAL = google.maps.UnitSystem.IMPERIAL;
    METRIC = google.maps.UnitSystem.METRIC;
    
    set.forEach(function (element) {
        originsSameDestinations.push(new google.maps.LatLng(element.lat, element.lng));
        path.push({ 'rank': element.rank, 'order': element.rank });
    });
    console.log(set);
}

var infowindowOfRoute;
function calculateAndDisplayRoute(directionsService, directionsDisplay, start, end, markerArray, stepDisplay, map) {
    // First, remove any existing markers from the map.
    for (var i = 0; i < markerArray.length; i++) {
        markerArray[i].setMap(null);
    }

    // Retrieve the start and end locations and create a DirectionsRequest using
    // WALKING directions.
    directionsService.route({
        origin: start,
        destination: end,
        travelMode: query.travelMode
    }, function(response, status) {
        // Route the directions and pass the response to a function to create
        // markers for each step.
        if (status === 'OK') {
            directionsDisplay.setDirections(response);
            var center_point = response.routes[0].overview_path.length / 2;
            if (infowindowOfRoute != null) {
                infowindowOfRoute.close();
            }
            infowindowOfRoute = new google.maps.InfoWindow();
            infowindowOfRoute.setContent(response.routes[0].legs[0].distance.text + "<br>" + response.routes[0].legs[0].duration.text + " ");
            infowindowOfRoute.setPosition(response.routes[0].overview_path[center_point | 0]);
            infowindowOfRoute.open(map);
            
            showSteps(response, markerArray, stepDisplay, map);
        } else {
            window.alert('Directions request failed due to ' + status);
        }
    });
}
function showSteps(directionResult, markerArray, stepDisplay, map) {
    // For each step, place a marker, and add the text to the marker's infowindow.
    // Also attach the marker to an array so we can keep track of it and remove it
    // when calculating new routes.
    var myRoute = directionResult.routes[0].legs[0];
    for (var i = 0; i < myRoute.steps.length; i++) {
        var marker = markerArray[i] = markerArray[i] || new google.maps.Marker;
        marker.setMap(map);
        //marker.setIcon
        marker.setPosition(myRoute.steps[i].start_location);
        attachInstructionText(stepDisplay, marker, myRoute.steps[i].instructions, map);
    }
}

function attachInstructionText(stepDisplay, marker, text, map) {
    google.maps.event.addListener(marker, 'click', function() {
        // Open an info window when the marker is clicked on, containing the text
        // of the step.
        stepDisplay.setContent(text);
        stepDisplay.open(map, marker);
    });
}

var query = {
    origins: originsSameDestinations,
    destinations: originsSameDestinations,
    travelMode: 'WALKING',
    unitSystem: METRIC
};

var map, dms;
var dirService, dirRenderer;
var highlightedCell;
var routeQuery;
var bounds;
var panning = false;
    
function initialize() {
    dms = new google.maps.DistanceMatrixService();
    dirService = new google.maps.DirectionsService();
    dirRenderer = new google.maps.DirectionsRenderer({preserveViewport:true});
    dirRenderer.setMap(map);

    google.maps.event.addListener(map, 'idle', function() {
        if (panning) {
            map.fitBounds(bounds);
            panning = false;
        }
    });
    updateMatrix();
}
    
function updateMatrix() {
    dms.getDistanceMatrix(query, function(response, status) {
        if (status == "OK") {
            var costMatrix = [];
            for (var i = 0; i < originsSameDestinations.length; i++) {
                var row = [];
                for (var j = 0; j < originsSameDestinations.length; j++) {
                    var element = response.rows[i].elements[j].distance.text;
                    row.push(parseFloat(element.substring(0 ,element.length-2)) * (element.substring(element.length-2, element.length).toLowerCase() == "km" ? 1000 : 1 ));
                }
                costMatrix.push(row);
            }
            result = NearsetNeighborTSP(costMatrix);
            for (var i = 0; i < path.length; i++) {
                for (var j = 0; j < result.length; j++) {
                    if (path[i]['rank'] == result[j]) {
                        path[i]['order'] = j+1;
                        break;
                    }
                }                
            }
            console.log(path);
            console.log(NearsetNeighborTSP(costMatrix));
        }
    });
}


function updateMode() {
    switch (document.getElementById("mode").value) {
        case "driving":
            query.travelMode = google.maps.TravelMode.DRIVING;
            break;
        case "walking":
            query.travelMode = google.maps.TravelMode.WALKING;
            break;
    }
    updateMatrix();
    if (routeQuery) {
        routeQuery.travelMode = query.travelMode;
    }
}

