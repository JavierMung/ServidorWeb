import { useEffect, useState } from 'react';
import '../styles/body-style.css'
import { AgregarAlCarrito, EliminarAritculos } from '../helpers/EndPoints';
function Card({ num, articulo, carrito }) {
    const [counter, setCounter] = useState(1)
    const [message, setMessage] = useState("")
    const [error, setError] = useState(false)
    const [success, setSuccess] = useState(false)
    useEffect(() => {
        if (carrito)
            setCounter(articulo.cantidad)
    }, [])

    const addCarrito = () => {
        console.log(articulo.Id, counter);

        fetch(AgregarAlCarrito, {
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
                Id: articulo.Id,
                cantidad: counter,
            }
            ), // body data type must match "Content-Type" header
        })
            .then(res => {
                console.log(res.status);
                if (res.status !== 200) {
                    return res.json().then(data => {
                        throw new Error(data.message);
                    });
                }
                return res.json();
            })
            .then(res => {
                setSuccess(true);
                setMessage(res.message);
            })
            .catch(err => {
                setError(true);
                setMessage(err.message);
                console.log(err);
            });


    }
    const delCarrito = (event) => {
        event.preventDefault()
        console.log(counter, articulo.Id);
        fetch(EliminarAritculos, {
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
            body: JSON.stringify({ cantidad: counter, Id: articulo.Id })
        }).then(res => {
            if (res.status !== 200) {
                console.log(res.json())
                throw new Error('Error en la transacción')
            }
            alert("OK")
            setCounter(1)
        })
            .catch(error => { setCounter(1); alert(error) })

    }


    const handlerPlus = () => {
        if (carrito && counter < articulo.cantidad)
            setCounter(counter + 1)
        else if (!carrito)
            setCounter(counter + 1)
    }
    const handlerMinor = () => {
        if (counter > 1)
            setCounter(counter - 1);
    }

    const formatter = new Intl.NumberFormat('es-MX', {
        style: 'currency',
        currency: 'MXN',

        // These options are needed to round to whole numbers if that's what you want.
        //minimumFractionDigits: 0, // (this suffices for whole numbers, but will print 2500.10 as $2,500.1)
        //maximumFractionDigits: 0, // (causes 2500.99 to be primessa
    });


    return (
        <div className='box-content-card'>

            <div className="card-articulo ">
                {
                    success ? <div style={{ width: "100%" }} class="alert alert-success alert-dismissible fade show" role="alert">
                        <strong>!Agregada con exito! </strong> revisa tu carrito.
                        <button type="button" class="btn-close" data-bs-dismiss="alert" onClick={() => setSuccess(false)}></button>
                    </div> : <></>
                }
                {
                    error ? <div class="alert alert-warning alert-dismissible fade show" role="alert">
                        <strong>!Error!</strong> Hubo un error: {message}
                        <button type="button" class="btn-close" data-bs-dismiss="alert" onClick={() => setError(false)}></button>
                    </div> : <></>
                }
                <div>
                    <h3 className='text-center'>{(articulo.Nombre).toUpperCase()}</h3>
                </div>
                <div className="mt-2 text-center">  {articulo.Foto && (
                    <div className="mt-2 text-center">
                        <img
                            src={`data:image/jpeg;base64,${articulo.Foto}`}
                            alt="Vista previa de la imagen"
                            width={150}
                            height={150}
                        />
                    </div>
                )}</div>
                {carrito ? <div>Subtotal: {formatter.format(articulo.Precio * articulo.cantidad)} MXN</div> : <p>Precio: {formatter.format(articulo.Precio)}</p>}
                {carrito ? <p>Numero de articulos: {articulo.cantidad}</p> : <></>}
                <button type="button" class="btn btn-primary btn-sm " data-bs-toggle="modal" data-bs-target={"#exampleModal" + num}>
                    ver descripcion
                </button>
                {carrito ? <button data-bs-toggle="modal" data-bs-target={"#exampleModal" + articulo.id} onClick={() => { console.log(articulo.id) }} class="btn btn-danger btn-sm mt-2">Eliminar del carrito</button> : <button onClick={addCarrito} class="btn btn-success btn-sm mt-2">Agregar al carrito</button>}
                <div className='botones-box' >
                    <div className='boton-box'>
                        <button className='boton' onClick={handlerPlus}>+</button>
                    </div>
                    <div className='boton-box'>
                        <button className='boton' onClick={handlerMinor}>-</button>
                    </div>
                    <div>{counter}</div>
                </div>

                {carrito ?
                    <>
                        <div class="modal fade" id={"exampleModal" + articulo.id} tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
                            <div class="modal-dialog">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title" id="exampleModalLabel">Eliminar articulo</h5>
                                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                    </div>
                                    <div class="modal-body">
                                        ¿Deseas eliminar el articulo?
                                    </div>
                                    <div class="modal-footer">
                                        <button type="button" class="btn btn-danger" data-bs-dismiss="modal">Cancelar</button>
                                        <button type="button" onClick={(event)=>delCarrito(event)} class="btn btn-success" data-bs-dismiss="modal">Aceptar</button>
                                    </div>
                                </div>
                            </div>
                        </div></>
                    : <></>}

            </div>
            <div class="modal fade" id={"exampleModal" + num} tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h1 class="modal-title fs-5" id="exampleModalLabel">{articulo.Nombre}</h1>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <h2>{articulo.Descripcion.toUpperCase()}</h2>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-danger" data-bs-dismiss="modal">cerrar</button>
                        </div>
                    </div>
                </div>
            </div>

        </div>);
}

export default Card;