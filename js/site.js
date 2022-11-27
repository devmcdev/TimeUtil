window.parseCSV = async (streamReference) => {
    const arrayBuffer = await streamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const file = new File([blob], 'csvfile');

    const parseRes = await parseCSVAsync(file);
    const events = parseEvents(parseRes.data);

    return JSON.stringify(events);
}

window.exportCSV = (eventsJson, fileName) => {
    const data = JSON.parse(eventsJson);

    for(let i = 0; i < data.length; i++){
        data[i].Categories = data[i].Categories.join(";");
    }

    const unParseRes = unParseCSV(data);

    const blob = new Blob([unParseRes],{
        type: "text/csv"
    });

    const url = URL.createObjectURL(blob);
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName;
    anchorElement.click();
    anchorElement.remove();
    URL.revokeObjectURL(url);
}

function unParseCSV(data) {
    const csvParseOptions = {
        header: true,
        skipEmptyLines: true,
    };

    return Papa.unparse(data, csvParseOptions);
}


function parseCSVAsync(file) {
    return new Promise((resolve, reject) => {
        const csvParseOptions = {
            complete: (result) => resolve(result),
            header: true,
            skipEmptyLines: true,
            error: (error) => reject(error)
        };
        Papa.parse(file, csvParseOptions);
    });
}

function parseEvents(data) {
    return data.map(x => {
        return {
            "Subject": x.Subject,
            "Start Date": x["Start Date"],
            "End Date": x["End Date"],
            "Start Time": x["Start Time"],
            "End Time": x["End Time"],
            categories: categoiresConverter(x.Categories),
        };
    })
}

function categoiresConverter(sourceCategorie) {
    if (!sourceCategorie) {
        return ["Uncategorised"];
    }
    return sourceCategorie.split(';');
}