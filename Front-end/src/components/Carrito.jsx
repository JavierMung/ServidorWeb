import Card from './Card'
import '../styles/body-style.css'
import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { ObtenerArticulosCarrito } from '../helpers/EndPoints';
function Carrito() {

    const [productos, setProductos] = useState([])
    const [total, setTotal] = useState(0);

    useEffect(() => {
        // Calculamos la suma total cada vez que el array de productos cambie
        let sumaTotal = productos.reduce((acumulador, producto) => {
            let cantidad = producto.cantidad;
            let precio = producto.Precio;
            let subtotal = cantidad * precio;
            return acumulador + subtotal;
        }, 0);
        setTotal(sumaTotal);
    }, [productos]);

    useEffect(() => {
        // Update the document title using the browser API
        fetch(ObtenerArticulosCarrito)
            .then(res => res.json())
            .then(data => {
                setProductos(data)
                console.log(data)
            })
            .catch(err => console.log(err))
    }, []);

    return (<>

        <div className='header-title'>
            <div>
                <h1>Articulos en el Carrito</h1>
            </div>

        </div>
        <div className='header-title2 '>
            <div className='btn btn-primary text-light '>
                <Link className='Link' to={"/productos"}>Seguir Comprando</Link>
            </div>
        </div >

        <section className='productos-box '>
            {!productos.length ?
                <>
                    <div class="spinner-border m-5" role="status">
                    </div>
                </> : <>
                    {productos.map((producto, index) => {
                        return (<>
                            <Card key={index} num={index} carrito={true} articulo={producto} />
                        </>
                        )
                    })}
                    <p>Suma total: {total}</p></>
            }

        </section>

    </>);
}

export default Carrito;