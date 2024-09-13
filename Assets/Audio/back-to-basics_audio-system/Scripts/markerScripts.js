// https://qa.fmod.com/t/can-you-access-a-list-of-markers-at-runtime/11819/10

function exportMarkers(success) {
    // Get all events
    var events = studio.project.model.Event.findInstances()

    var data = {};
    data.events = [];
    
    // See a list of events
    for(var i = 0; i < events.length; i++) {
        var eventPath = events[i].getPath();
        eventData = {};
        eventData["eventPath"] = eventPath;

        studio.system.warn(eventPath);

        var markers = []

        for (var j = 0; j < events[i].markerTracks.length; j++) {
            for (var k = 0; k < events[i].markerTracks[j].markers.length; k++) {

                var marker = events[i].markerTracks[j].markers[k];

                if (marker.entity !== "TempoMarker" ) { continue; }

                markerData = {
                    "id": marker.id,
                    "positionMs": marker.position * 1000,
                    "tempoBpm": marker.tempo,
                    "timeSignatureNumerator": marker.timeSignatureNumerator,
                    "timeSignatureDenominator": marker.timeSignatureDenominator
                }

                markers.push(markerData)

                // position: 8 <-- this is in seconds!
                // tempo: 100, <-- this is in bpm
                // timeSignatureNumerator: 4, <-- this is integer
                // timeSignatureDenominator: 4, <-- this is integer
            }
        }

        // sort markers by timeline position (assume one timeline!) and 
        function predicate(a,b) {
            return a.positionMs - b.positionMs;
        }
        markers.sort(predicate);
        
        eventData.markers = markers;

        data.events.push(eventData);
    }

    var outputPath = studio.project.filePath+'/../markerDump.json';
    var file = studio.system.getFile(outputPath);
    file.open(studio.system.openMode.WriteOnly);
    file.writeText(JSON.stringify(data));
    file.close();
}

studio.project.buildStarted.connect(exportMarkers);

/*
	-------------------------------------------
	Exporting Markers
	-------------------------------------------
	!!!ATTENTION!!! 
	This script modifies your markers in order to sort them by time, this may not be in your interest.
	look further down the script for the "!!!ATTENTION!!!" comment.
	
	This Script was written for Fmod Studio 2.00.01
	it exports markers into a single csv file that can be read by Unreal Engine 4.22 into a Data table
	
	The data struct inside of the Unreal Engine to be linked should contain:
	IsActive(bool),Name(string),Start(integer),Position(integer),End(integer)
	
	the script awaits that every event track is dedicated to a single instrument,
	if it finds a single loop, transition or anything that has an undefined name,
	it will consider that event track not meant to be written into the CSV file.
*/


// studio.menu.addMenuItem({ name: "Markers To File",
// 	// if no event is selectd, the script can not be executed
// 	isEnabled: function() { var event = studio.window.browserCurrent(); return event && event.isOfExactType("Event"); },
// 	keySequence: "Alt+E",
// 	execute: function() {
// 		//get the current selected event
// 		var event = studio.window.browserCurrent();

// 		//get all events and not only the selectd one
// 		//var events = studio.project.model.event.findInstances();

// 		//set CSV table column structure as first line (--- is the column header for the ID of every row in UE4)
// 		var FinalData = "---,Position,Name";
// 		var InstrumentData="";
// 		var i=0, start, position, end
// 		var timemargin=200;	//can be adjusted, will be used to calculate start and end
// 		//because UE4 awaits the values to be set in quotes and Jscript doesn't accept """ as string
// 		var quotes = String.fromCharCode(34);
		
// 		//console can be opened with ctl+0 in fmod studio 
// 		//console.log("############Sort#################");
		
// 		// Sort markers by time/position
// 		/* 
// 		-------------------------------------------
// 		##############!!!ATTENTION!!!##############
// 		-------------------------------------------
// 		this process edits the marker data in the Fmod Project
// 		it only swaps the positions and names of the markers, other data stays with the individual marker
// 		This is a simple bubble sort
// 		as default markers are in the markers array as they were created and not where they sit on the timeline
// 		*/
// 		for (x = 0; x < event.markerTracks.length; x++){
// 			var sorting = true;
// 			var tempP, tempN;
			
// 			while (sorting){
// 				sorting = false;
				
// 				for (y = 0; y+1 < event.markerTracks[x].markers.length; y++){
// 					//need a swap?
// 					if(event.markerTracks[x].markers[y].position > event.markerTracks[x].markers[y+1].position){
// 						//set sort to true for while loop
// 						sorting=true;
// 						// save value and name
// 						tempP = event.markerTracks[x].markers[y].position;
// 						tempN = event.markerTracks[x].markers[y].name;
// 						//interchange values and names
// 						event.markerTracks[x].markers[y].position = event.markerTracks[x].markers[y+1].position;
// 						event.markerTracks[x].markers[y].name = event.markerTracks[x].markers[y+1].name;
// 						event.markerTracks[x].markers[y+1].position = tempP;
// 						event.markerTracks[x].markers[y+1].name = tempN;
// 					}
// 				}

// 			}
// 		}
		
// 		//console can be opened with ctl+0 in fmod studio 
// 		//console.log("############Sort Finished#################");
		
		
// 		/*
// 		the writing process is seperated into FinalData and InstrumentData because the script will stop
// 		writing to InstrumentData if there is a single loop, transition or other stuff without a name
// 		in the event track, each event track shall only contain markers
// 		*/		
// 		for (x = 0; x < event.markerTracks.length; x++){
// 			for (y = 0; y < event.markerTracks[x].markers.length; y++){
// 				//loops,transistions and transition regions have an undefined name
// 				if(event.markerTracks[x].markers[y].name == undefined){
// 					InstrumentData="";	//delete possible written track markers
// 					//break for loop, because the event track contains loops,transitions,...
// 					break;
// 				}
// 				else{
// 					//recalculate position to a full millisecend format (as fmod puts that out)
// 					position = Math.round(event.markerTracks[x].markers[y].position*1000)
										
// 					//Add the marker time position and name to the InstrumentData variable
// 					InstrumentData += "\r\n" + i + "," + quotes + position + quotes + "," + quotes + event.markerTracks[x].markers[y].name + quotes;
// 					//index for the csv file, every line needs a unique ID
// 					i=i+1;
// 				}
// 			}
// 		//add line of data to variable that get written into the csv file
// 		FinalData += InstrumentData;
// 		//clear for next line of data
// 		InstrumentData="";
// 		}
		
// 		//File output, file can be found in the fmod studio install directory
// 		var file = studio.system.getFile("./"+event.name+".csv");
// 		file.open(studio.system.openMode.WriteOnly);
// 		file.writeText(FinalData);
// 		file.close();

// 	}
// });