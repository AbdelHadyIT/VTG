
//begin Nearest Neighbor TSP Algorithm 
function max(array) {
    var maximum = array[0][0];
    for (var i = 0; i < array.length; i++) {
        for (var j = 0; j < array.length; j++) {
            if (array[i][j] > maximum) {
                maximum = array[i][j];
            }
        }
    }
    return maximum;
}

function min(array) {
    var minimum = array[0][0].valueOf();
    var edge = { 'from': 0, 'to': 0 };
    for (var i = 0; i < array.length; i++) {
        for (var j = 0; j < array.length; j++) {
            if (array[i][j] < minimum) {
                minimum = array[i][j].valueOf();
                edge['from'] = i.valueOf(), edge['to'] = j.valueOf();
            }
        }
    }
    return edge;
}

function finite(ii, jj, cost, infinity) {
    var costAfterFinite = [];
    for (var i = 0; i < cost.length ; i++) {
        var row = [];
        for (var j = 0 ; j < cost.length; j++) {
            if (i == ii || j == jj || i == jj && j == ii) {
                row.push(parseFloat(infinity.toString()));
            } else {
                row.push(parseFloat(cost[i][j].toString()));
            }
        }
        costAfterFinite.push(row);
    }
    return costAfterFinite;
}

function finalTravelWithoutVisitAll(from, to, cost, infinity) {
    var costAfterFinite = finite(from, to, cost, infinity);
    var valid = true;
    for (var i = 0; i < cost.length; i++) {
        for (var j = 0; j < cost.length; j++) {
            if (costAfterFinite[i][j] != infinity) {
                valid = false;
                break;
            }
        }
    }
    return valid;
}

function NearsetNeighborTSP(cost) {
    var infinity = max(cost) + 1000;
    for (var i = 0; i < cost.length; i++) {
        cost[i][i] = infinity;
    }
    if (cost.length < 3) {
        edge = min(cost);
        return [edge['from']+1, edge['to']+1, edge['from']+1];
    } else {
        
        var edges = [];
        for (var i = 0; i < cost.length; i++) {
            var edge = min(cost);
            while (true) {
                if ((i != cost.length - 1) && finalTravelWithoutVisitAll(edge['from'], edge['to'], cost, infinity)) {
                    cost[edge['from']][edge['to']] = infinity;
                    edge = min(cost);
                } else {
                    break;
                }
            }
            edges.push(edge);
            cost = finite(edge['from'], edge['to'], cost, infinity);
        }
        var path = [];
        path.push(edges[0]['from']);
        path.push(edges[0]['to']);
        for (var i = 1; i < cost.length; i++) {
            for (var j = 1; j < cost.length; j++) {
                if (edges[j]['from'] == path[i]) {
                    path.push(edges[j]['to']); break;
                }
            }
        }
        for (var i = 0; i < path.length; i++) {
            path[i]++;
        }
        return path;
    }
}
//end Nearest Neighbor TSP Algorithm 