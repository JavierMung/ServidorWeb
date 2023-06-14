import { Link } from 'react-router-dom';
import Card from './Card'
import '../styles/body-style.css'
import { useEffect, useState } from 'react';
import { ObtenerArticulos } from '../helpers/EndPoints';
import { BuscarArticulo } from '../helpers/EndPoints';
function Articulos() {
    const [busqueda, setBusqueda] = useState({ busqueda: "" })
    const [productos, setProductos] = useState([])



    const handleChange = (e) => {
        // 👇 Store the input value to local state
        setBusqueda({ busqueda: e.target.value });
    };

    const handlerBusqueda = () => {

        fetch(BuscarArticulo+"busqueda="+busqueda.busqueda)
            .then(res => {
                if (res.status !== 200) {
                    setProductos([])
                    throw new Error('promise chain cancelled');
                }
                return res.json()
            })
            .then(data => {
                setProductos(data)
                console.log(data)
            })
            .catch(err => console.log(err))
    }
    const handleKeyDown = (event) => {
        if (event.key === 'Enter') {
            handlerBusqueda()
        }
    };

    useEffect(() => {
        // Update the document title using the browser API
        fetch(ObtenerArticulos)
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
                <h1>Articulos</h1>
            </div>

        </div>
        <div className='header-title'>
            <div>
                <input onKeyDown={handleKeyDown} onChange={handleChange} value={busqueda.busqueda} placeholder='Eje. Pantalon...' />
                <button onClick={handlerBusqueda}>Buscar</button>
            </div>
        </div>
        <div className='header-title2 mt-3'>
            <div className='btn btn-primary text-light '>
                <Link className='Link' to={"/carrito"}>Carrito</Link>
            </div>
        </div >
        <section className='productos-box '>

            {!productos.length ?
                <>
                    
                    <div class="spinner-border m-5" role="status">
                    </div>
                </> : <>{productos.map((producto, index) => {
                    return (<>
                        <Card key={index} num={index} articulo={producto} />
                    </>
                    )
                })}</>}

        </section>
    </>);
}

export default Articulos;