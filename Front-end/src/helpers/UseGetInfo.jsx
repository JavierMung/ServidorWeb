import React, { useEffect, useState } from "react";

const articuloContext = React.createContext()
const setArticulosContext = React.createContext()

export function UseGetInfo() {

    const [articulos, setArticulos] = useState([])

    useEffect(() => {
        // Update the document title using the browser API
        fetch('http://20.97.214.185:8080/Servicio/rest/ws/consulta_articulo', {
            method: "POST", // *GET, POST, PUT, DELETE, etc.
            mode: "cors", // no-cors, *cors, same-origin
            cache: "no-cache", // *default, no-cache, reload, force-cache, only-if-cached
            credentials: "same-origin", // include, *same-origin, omit
            headers: {
                "Content-Type": "application/json",
                // 'Content-Type': 'application/x-www-form-urlencoded',
            },
            redirect: "follow", // manual, *follow, error
            referrerPolicy: "no-referrer", // no-referrer, *no-referrer-when-downgrade, origin, origin-when-cross-origin, same-origin, strict-origin, strict-origin-when-cross-origin, unsafe-url
            body: JSON.stringify(busqueda), // body data type must match "Content-Type" header
        })
            .then(res => res.json())
            .then(data => {
                setProductos(data)
                console.log(data)
            })
            .catch(err => console.log(err))
    }, []);

   // const getInfo()

}