$(document).ready(function () {

    $("#tl-search-providers_old").click(function () {
        event.stopPropagation();

        $("#tl-provider-search-status").html("searching...");
        $('#tl-provider-search-status').removeClass("tl-hidden");

        //TODO: Get postcode from form
        const postcode = 'OX2 9GX';
        //let webApiUrl = 'https://localhost:58879/api/search';
        let webApiUrl = $('#searchApiUrlValue').val();
        console.log(`webApiUrl is ${webApiUrl}`);

        //Get qualifications list
        //https://localhost:55961/FindProviders/api/qualifications
        const qualificationsRequest = new Request(`${webApiUrl}/qualifications`);
        fetch(qualificationsRequest)
            .then(response => response.json())
            .then(function(data) {
                console.log('QualificationsRequest succeeded with JSON response:', data);
                populateQualificationsResults(data);
            }).catch(function(error) {
                console.log('Request failed:', error.message);
            });

        /*
        const search = new URLSearchParams({
            postcode: postcode,
        }).toString();
                
        console.log(`Ready to call ${webApiUrl}?${search}`);

        const request = new Request(`${webApiUrl}?${search}`);

        fetch(request)
            .then(response => response.json())
            //.then(data => console.log(data));
            .then(function (data) {
                console.log('Request succeeded with JSON response:', data);
                populateSearchResults(data);
            }).catch(function (error) {
                console.log('Request failed:', error.message);
            })
        */
    });

    function populateQualificationsResults(data) {
        const select = $("#tl-qualifications-select");
        select.children().remove();

        data.forEach(function (item) {
            console.log(`${item.id} - ${item.name}`);
            var opt = new Option(item.name, item.id);
            $("#tl-qualifications-select").append(opt);
        });
    }

    function populateSearchResults(data) {

        $("#tl-provider-search-status").html("loading...");

        $("#tl-provider-search-results-json").html(JSON.stringify(data, undefined, 2));

        let searchResults =
            '<div class="tl-results"> \
            <p>Qualifications:</p> \
            <ul>';

        //List qualifications
        data.qualifications.forEach(function (item) {
            searchResults += `<li>${item.text} (${item.value})</li>`
        });

        searchResults +=
            '</ul> \
         <p>Providers:</p> \
            <ul>';

        data.providerLocations.forEach(function (item) {
            searchResults +=
                `<div class="tl-results-box"> \
                <h3><span class="tl-results-box--distance">${item.distanceInMiles} miles</span> \
                    ${item.providerName}</h3> \
                    <h4>${item.postcode}</h4>`;

            item.deliveryYears.forEach(function (year) {
                searchResults +=
                    `<h5>From ${year.year}</h5> \
                   <ul>`;

                year.qualifications.forEach(function (qualification) {
                    searchResults +=
                        `<li>${qualification.name}</li>`;
                });

                searchResults += '</ul>';
            });

            searchResults +=
                `<a target="_" href='${item.website}' class='tl-link'>Go to provider website</a> \
             <a target="_" href='${item.journeyUrl}' class='tl-link'>How to get there</a>`;

            searchResults += '</div >';
        });

        searchResults += '</ul">'

        searchResults += '</div">'

        console.log(searchResults);

        $("#tl-provider-search-results").empty();
        $("#tl-provider-search-results").append(searchResults);

        $("#tl-provider-search-status").html("");
        $('#tl-provider-search-status').addClass("tl-hidden");
    }
});
