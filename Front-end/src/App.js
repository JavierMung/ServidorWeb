import { Route, Routes } from 'react-router-dom';
import Menu from './components/Menu';
import './styles/header-style.css'
import Articulos from './components/Articulos';
import Alta from './components/Alta';
import Carrito from './components/Carrito';


function App() {
  return (
    <div className="main-content-box ">
      <section className="body-content-box ">
        <Menu />
      </section>
      
      <div className='articulos-content-box' >
        <Routes>
          <Route path="/" element={<Alta />} />
          <Route path="/productos" element={<Articulos />} />
          <Route path="/carrito" element={<Carrito />} />
        </Routes>
      </div>

    </div>
  );
}

export default App;
