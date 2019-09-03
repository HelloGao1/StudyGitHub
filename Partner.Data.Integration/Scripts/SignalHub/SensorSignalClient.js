//
// Corporate with Index.html file.
//
var connection = $.hubConnection();
var sensorDataHubProxy = connection.createHubProxy('sensorDataHub');
var stateConversion = { 0: 'connecting', 1: 'connected', 2: 'reconnecting', 4: 'disconnected' };
var currentPatientId = '';
var currentConnectionState = 'disconnected';
var isFirstMeasureData = false;
var selectedMeasureIndex = 0;
var previousSelectdMeasureIndex = 0;

//
// Receive signalr hub broad casting patient's sensor data.
//
sensorDataHubProxy.on('broadCastSensorData', function (sensorData) {
    // console.log('sensorData: ' + JSON.stringify(sensorData));
   
    if (sensorData && sensorData.PatientId === currentPatientId) {
      
        if (isFirstMeasureData) {
            //
            // Rendering the measure data options.
            //
            if (sensorData.Measures && sensorData.Measures.length > 0) {

                renderMeasures(sensorData.Measures);
                isFirstMeasureData = false;
               // cleanupChart();
            }
        }
        else {

            if (previousSelectdMeasureIndex !== selectedMeasureIndex) {
                previousSelectdMeasureIndex = selectedMeasureIndex;
               // cleanupChart();
            }

            //
            // Push sensor data to chart. Work for chart signal rendering.
            //
            pushDataToChart(sensorData.Measures[selectedMeasureIndex].Value);
        }
    }
});

connection.stateChanged(connectionStateChanged);

function connectPatient() {

    if (currentConnectionState === 'disconnected') {
        console.log('Disconnected');
        console.log(connection);
        currentPatientId = $("#txtPatientNumber").val();
        connection.start()
            .done(function () {
                // console.log('Now connected, connection ID=' + connection.id);
                sensorDataHubProxy.invoke("joinPatient", currentPatientId);
                isFirstMeasureData = true;
                $('#patientModal').modal('hide');
                toastr.success("Sytem successfully connects to patient: " + currentPatientId + " sensor data channel.");
            })
            .fail(function (ex) {
                console.log(ex);
                // console.log('Could not connect');
                toastr.error("Sytem cannot connect to patient's sensor data channel.");
            });
    }
    else {
        console.log('Already Connected');
        // leave the original join patient.
        if (currentPatientId && currentPatientId.length > 0) {
            console.log('leave patient: ' + currentPatientId);
            sensorDataHubProxy.invoke("leavePatient", currentPatientId);
            isFirstMeasureData = true;
        }

        currentPatientId = $("#txtPatientNumber").val();
        sensorDataHubProxy.invoke("joinPatient", currentPatientId);

        $('#patientModal').modal('hide');
        toastr.success("Sytem successfully connects to patient: " + currentPatientId + " sensor data channel.");
    }

    $("#txtCurrentPatientId").html(currentPatientId);
}

function connectionStateChanged(state) {
    console.log('SignalR state changed from: ' + stateConversion[state.oldState]
        + ' to: ' + stateConversion[state.newState]);
    currentConnectionState = stateConversion[state.newState];
}

function renderMeasures(measures) {
    var result = '<ul class="list-group">';
    if (measures && measures.length > 0) {
        $.each(measures, function (key, measure) {
            // console.log('each called' + key);
            result += '<li class="list-group-item">' + "<input type='radio' name='measureOption' onChange='setupSelectedMeasureIndex()' value='" + key + "' " + checkSelectedRadio(key) +" />" + measure.Name + " " + measure.Format + " (" + measure.Unit + ')</li>';
        });
    }
    result += '</ul>';
    $("#measuresGroup").html(result);
}

function setupSelectedMeasureIndex() {
    selectedMeasureIndex = $("input[name='measureOption']:checked").val();
    // console.log('selectedMeasureIndex: ' + selectedMeasureIndex);
    // cleanupChart();
    initChart();
}

function checkSelectedRadio(index) {
    // console.log('checkSelectedRadio called.' + index);

    if (selectedMeasureIndex === index) {
        return "checked='checked'";
    } else {
        return "";
    }
}