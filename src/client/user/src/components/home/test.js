async function getEmployees() {
    const endpoint = 'http://dummy.restapiexample.com/api/v1/employees';
    // bu api'den bir çok kez 429 hatası aldığımdan dolayı bu şekilde test ettim.
    // const test = {
    //     status: "success",
    //     data: [
    //         {
    //             "id": 1,
    //             "employee_name": "Tiger Nixon",
    //             "employee_salary": 320800,
    //             "employee_age": 61,
    //             "profile_image": ""
    //         },
    //         {
    //             "id": 2,
    //             "employee_name": "Garrett Winters",
    //             "employee_salary": 170750,
    //             "employee_age": 63,
    //             "profile_image": ""
    //         }]};
    // return test.data;
    var response = await fetch(endpoint)
        .then(async resp => {
            var jsonResponse = await resp.json();
            if (!jsonResponse || jsonResponse.status !== 'success')
                throw new Error(`Api isteğinde hata oluştu: (${jsonResponse.status}: ${jsonResponse.message}))`);
            return jsonResponse.data;
        })
        .catch(err => {
            console.log(err);
            throw new Error(`Api isteği sırasında hata oluştu: (${err.message})`);
        });

    return response;
}