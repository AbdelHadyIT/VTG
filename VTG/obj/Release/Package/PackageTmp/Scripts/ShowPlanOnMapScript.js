//$(document).ready(console.log(NearsetNeighborTSP([
//                [0, 35, 70, 105, 165],
//                [35, 0, 45, 20, 80],
//                [70, 45, 0, 30, 75],
//                [105, 20, 30, 0, 60],
//                [165, 80, 75, 60, 0]])));
function showToast(text) {
    // Get the snackbar DIV
    var x = document.getElementById("snackbar");
    x.innerText = text;
    // Add the "show" class to DIV
    x.className = "show";
    // After 3 seconds, remove the show class from DIV
    setTimeout(function () { x.className = x.className.replace("show", ""); }, 3000);
}
var map, distanceMatrixSrevice;
var directionsRender;
var directionsDisplay;
var directionsService;
var map;
var IMPERIAL;
var METRIC;
var markersOfSteps = [];
var infowindowOfRoute;
var originsSameDestinations = [];
var infoWindow;

function CenterControl(controlDiv, map) {

    // Set CSS for the control border.
    var controlUI = document.createElement('div');
    controlUI.style.backgroundColor = '#fff';
    controlUI.style.border = '2px solid #fff';
    controlUI.style.borderRadius = '3px';
    controlUI.style.boxShadow = '0 2px 6px rgba(0,0,0,.3)';
    controlUI.style.cursor = 'pointer';
    controlUI.style.marginBottom = '22px';
    controlUI.style.textAlign = 'center';
    controlUI.title = 'Click to recenter the map';
    controlDiv.appendChild(controlUI);

    // Set CSS for the control interior.
    var controlText = document.createElement('div');
    controlText.style.color = 'rgb(25,25,25)';
    controlText.style.fontFamily = 'Roboto,Arial,sans-serif';
    controlText.style.fontSize = '16px';
    controlText.style.lineHeight = '38px';
    controlText.style.paddingLeft = '5px';
    controlText.style.paddingRight = '5px';
    controlText.innerHTML = 'Center Map';
    controlUI.appendChild(document.getElementById("controls"));
}
function initMap() {
    

    IMPERIAL = google.maps.UnitSystem.IMPERIAL;
    METRIC = google.maps.UnitSystem.METRIC;

    distanceMatrixSrevice = new google.maps.DistanceMatrixService();
    directionsService = new google.maps.DirectionsService;
    directionsRender = new google.maps.DirectionsRenderer({ preserveViewport: true });
    directionsRender.setMap(map);
    
    //-----------------------
    var latitude = 34.80208544142896; // YOUR LATITUDE VALUE
    var longitude = 38.996811152237456; // YOUR LONGITUDE VALUE

    var myLatLng = { lat: latitude, lng: longitude };

    
    map = new google.maps.Map(document.getElementById('map'), {
        center: myLatLng,
        zoom: 7,
        disableDoubleClickZoom: true, // disable the default map zoom on double click
    });
    var centerControlDiv = document.createElement('div');
    var centerControl = new CenterControl(centerControlDiv, map);

    centerControlDiv.index = 1;
    map.controls[google.maps.ControlPosition.TOP_CENTER].push(centerControlDiv);

    map.addListener("click", function (event) {
        if (directionsDisplay != null) {
            if (infowindowOfRoute != null) {
                infowindowOfRoute.setMap(null);
            }
            for (var i = 0; i < markersOfSteps.length; i++) {
                markersOfSteps[i].setMap(null);
            }
            directionsDisplay.setMap(null);
        }
    });

    updateMatrix();
        
            
}

function calculateAndDisplayRoute(directionsService, /*directionsDisplay,*/ start, end, markerArray, stepDisplay, map) {
    // First, remove any existing markersOfSteps from the map.
    for (var i = 0; i < markerArray.length; i++) {
        markerArray[i].setMap(null);
    }

    
    // Retrieve the start and end locations and create a DirectionsRequest using
    directionsService.route({
        origin: start,
        destination: end,
        travelMode: query.travelMode
    }, function(response, status) {
        // Route the directions and pass the response to a function to create
        // markersOfSteps for each step.
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
    
var countOfClicksOnMarkersToDetermineRoute = 0;
var start, end;
var markersOfPlan = [];
function updateMatrix() {
    if (directionsDisplay != null) {
        directionsDisplay.setMap(null);
    }
    for (var i = 0; i < markersOfSteps.length; i++) {
        markersOfSteps[i].setMap(null);
    }
    if (infowindowOfRoute != null) {
        infowindowOfRoute.setMap(null);
    }
    directionsDisplay = new google.maps.DirectionsRenderer({ map: map, suppressMarkers: false, suppressInfoWindows: true });
    for (var i = 0; i < markersOfPlan.length; i++) {
        markersOfPlan[i].setMap(null);
    }
    originsSameDestinations = [];
    set.forEach(function (element) {
        originsSameDestinations.push(new google.maps.LatLng(element.lat, element.lng));
    });
    query.origins = originsSameDestinations;
    query.destinations = originsSameDestinations;
    calculateDistance().then(function (response) {
            var costMatrix = [];
            for (var i = 0; i < originsSameDestinations.length; i++) {
                var row = [];
                for (var j = 0; j < originsSameDestinations.length; j++) {
                    var element = response.rows[i].elements[j].distance.text;
                    row.push(parseFloat(element.substring(0, element.length - 2)) * (element.substring(element.length - 2, element.length).toLowerCase() == "km" ? 1000 : 1));
                }
                costMatrix.push(row);
            }
            return costMatrix;
    }).done(function (costMatrix) {
        infoWindow = new google.maps.InfoWindow;
        var result = NearsetNeighborTSP(costMatrix);
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
            var icon = { label: result[item.rank - 1].toString() };
            var point = new google.maps.LatLng(
                        parseFloat(item.lat),
                        parseFloat(item.lng));
            var marker = new google.maps.Marker({
                map: map,
                position: point,
                label: icon.label
            });
            marker.addListener("rightclick", function (event) {
                if (infoWindow != null) {
                    infoWindow.close();
                }
                infoWindow.setContent(infowincontent);
                infoWindow.open(map, marker);
            });
            marker.addListener("click", function (event) {
                if (countOfClicksOnMarkersToDetermineRoute % 2 == 0) {
                    start = marker.getPosition().lat() + ', ' + marker.getPosition().lng();
                    countOfClicksOnMarkersToDetermineRoute++;
                    showToast("now select destination...");
                } else {
                    end = marker.getPosition().lat() + ', ' + marker.getPosition().lng();
                    if (start != end) {
                        calculateAndDisplayRoute(directionsService, /*directionsDisplay,*/ start, end, markersOfSteps, stepDisplay, map);
                        countOfClicksOnMarkersToDetermineRoute++;
                    }
                }
            });
            markersOfPlan.push(marker);
        });
    });
}

function calculateDistance() {
    var dfd = $.Deferred();
    distanceMatrixSrevice.getDistanceMatrix(query, function (response, status) {
        if (status == google.maps.DistanceMatrixStatus.OK)
            dfd.resolve(response);
        else
            dfd.reject(status);
    });
    return dfd.promise();
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
}

