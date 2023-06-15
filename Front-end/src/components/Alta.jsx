import { useState } from "react"
import { CapturaArticulo } from "../helpers/EndPoints"
//let foto = null

function Alta() {
    const [nombre, setNombre] = useState("")
    const [descripcion, setDescripcion] = useState("")
    const [cantidad, setCantidad] = useState(1)
    const [precio, setPrecio] = useState(1)
    const [foto, setFoto] = useState("")
    const [previewUrl, setPreviewUrl] = useState("");
    const [loading, setLoading] = useState(false)
    const handleChangeName = (e) => {
        // 👇 Store the input value to local state
        setNombre(e.target.value);
    };
    const handleChangeDescripcion = (e) => {
        // 👇 Store the input value to local state
        setDescripcion(e.target.value);
    };
    const handleChangeCantidad = (e) => {
        // 👇 Store the input value to local state
        setCantidad(e.target.value);
    };
    const handleChangePrecio = (e) => {
        // 👇 Store the input value to local state
        setPrecio(e.target.value);
    };

    const handleChangeFoto = (e) => {
        const file = e.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = (e) => {
                const base64String = btoa(
                    new Uint8Array(e.target.result).reduce(
                        (data, byte) => data + String.fromCharCode(byte),
                        ''
                    )
                );
                //                setFoto(base64String);
            };
            reader.readAsArrayBuffer(file);
        }
    };
    const handleFileChange = (e) => {
        const file = e.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                //foto = reader.result.split(',')[1];
                setFoto(reader.result.split(',')[1]);
                setPreviewUrl(reader.result);
            };
            reader.readAsDataURL(file);
        }
    };



    const alta = (event) => {
        event.preventDefault();
        setLoading(true)
        fetch(CapturaArticulo, {
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
            body: JSON.stringify({
                id: null,
                nombre: nombre,
                precio: precio,
                cantidad: cantidad,
                descripcion: descripcion,
                foto: foto
            }
            ), // body data type must match "Content-Type" header
        })
            .then(res => {

                if (res.status !== 200)
                    throw new Error("Error al enviar el formulario")
                return res.json()
            }).then(res => {
                foto = null
            })
            .catch(err => console.log(err))
            .finally(() => {
                setLoading(false)
                setPrecio(1)
                setCantidad(1)
                setDescripcion("")
                setFoto("")
                setNombre("")
                setPreviewUrl("")
            })
    }


    return (
        <div>
            <div className='header-title'>
                <div>
                    <h1>Captura de artículos</h1>
                </div>
            </div>
            <form >
                <div className="mt-2 text-center">  {foto && (
                    <div className="mt-2 text-center">
                        {previewUrl && (
                            <div>
                                <img width={150}
                                    height={150} src={previewUrl} alt="Vista previa de la imagen" />
                            </div>
                        )}
                    </div>
                )}</div>
                <div className="mt-2 text-center"><div>Nombre: </div><input onChange={handleChangeName} value={nombre} type="text" placeholder="Nombre" /></div>
                <div className="mt-2 text-center"><div>Descripcion:</div>  <input onChange={handleChangeDescripcion} value={descripcion} type="text" placeholder="Descripcion" /></div>
                <div className="mt-2 text-center"><div>Cantidad:</div> <input onChange={handleChangeCantidad} value={cantidad} type="number" placeholder="Cantidad" /></div>
                <div className="mt-2 text-center"><div>Precio:</div> <input onChange={handleChangePrecio} value={precio} type="number" placeholder="Precio" /></div>
                <div className="ps-5 mt-3 text-center"> <input type="file" accept="image/*" onChange={handleFileChange} /></div>
                <div className="mt-3 text-center "><button onClick={(event) => alta(event)} className="btn btn-success">Enviar</button></div>
                {loading ? <div className=" text-center mt-3">
                    <div>
                        Capturando articulo...
                    </div>
                    <div class=" spinner-border mt-3" role="status">
                    </div>
                </div> : <></>
                }
            </form>
        </div>);
}

export default Alta;