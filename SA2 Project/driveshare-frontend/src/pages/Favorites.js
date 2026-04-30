import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import useFavorites from '../hooks/useFavorites';

export default function Favorites() {
  const { user } = useAuth();
  const { favorites, toggleFavorite, isFavorite } = useFavorites();
  const navigate = useNavigate();

  if (!user) {
    return (
      <div className="container py-5 mt-4 text-center">
        <h2 className="fw-bold mb-4">My Favorites</h2>
        <p className="lead mb-4">Please log in to view your favorites.</p>
        <button className="btn btn-primary rounded-pill px-4" onClick={() => navigate('/login')}>
          Login
        </button>
      </div>
    );
  }

  const handleBookNow = (carId) => {
    navigate(`/booking/${carId}`);
  };

  return (
    <div className="container py-5 mt-4">
      <div className="mb-4">
        <h2 className="fw-bold m-0 border-start border-primary border-4 ps-3">My Favorites</h2>
        <p className="text-secondary mt-2 ms-3">{favorites.length} saved car{favorites.length !== 1 ? 's' : ''}</p>
      </div>

      {favorites.length === 0 ? (
        <div className="text-center py-5 bg-light rounded-4">
          <div className="display-1 mb-3">🤍</div>
          <h3 className="text-muted">No favorites yet</h3>
          <p className="text-secondary">Browse cars and tap the heart to save them.</p>
          <button className="btn btn-primary rounded-pill mt-3 px-4" onClick={() => navigate('/')}>
            Browse Cars
          </button>
        </div>
      ) : (
        <div className="row g-4">
          {favorites.map((car) => (
            <div className="col-md-6 col-lg-4" key={car.id}>
              <div className="card h-100 border-0 shadow-sm rounded-4 overflow-hidden fav-card">
                <div className="position-relative">
                  <img
                    src={car.imageUrl || "https://images.unsplash.com/photo-1533473359331-0135ef1b58bf"}
                    className="card-img-top"
                    alt={`${car.brand} ${car.model}`}
                    style={{ height: "220px", objectFit: "cover" }}
                  />
                  <button
                    onClick={(e) => {
                      e.stopPropagation();
                      toggleFavorite(car);
                    }}
                    style={{
                      position: 'absolute',
                      top: '12px',
                      left: '12px',
                      background: 'rgba(0,0,0,0.45)',
                      border: 'none',
                      borderRadius: '50%',
                      width: '36px',
                      height: '36px',
                      display: 'flex',
                      alignItems: 'center',
                      justifyContent: 'center',
                      cursor: 'pointer',
                      fontSize: '18px',
                      transition: 'transform 0.2s ease, background 0.2s ease',
                      zIndex: 10,
                    }}
                    onMouseEnter={e => e.currentTarget.style.transform = 'scale(1.15)'}
                    onMouseLeave={e => e.currentTarget.style.transform = 'scale(1)'}
                    title="Remove from favorites"
                  >
                    ❤️
                  </button>
                </div>

                <div className="card-body p-4">
                  <div className="d-flex justify-content-between align-items-start mb-2">
                    <div>
                      <h5 className="card-title fw-bold mb-1 fs-4">{car.brand} {car.model}</h5>
                      <p className="text-muted small mb-0">
                        <i className="bi bi-geo-alt-fill me-1"></i> {car.location} | {car.year}
                      </p>
                    </div>
                    <div className="text-end">
                      <span className="text-primary fw-bold fs-4">${car.pricePerDay}</span>
                      <span className="text-muted small">/day</span>
                    </div>
                  </div>

                  <div className="d-flex mt-3 mb-4 text-muted small gap-3">
                    <span><i className="bi bi-person me-1"></i> 5 Seats</span>
                    <span><i className="bi bi-gear me-1"></i> Auto</span>
                    <span><i className="bi bi-fuel-pump me-1"></i> Electric</span>
                  </div>

                  <div className="d-grid gap-2">
                    <button
                      onClick={() => handleBookNow(car.id)}
                      className="btn btn-primary rounded-pill py-2 fw-bold shadow-sm"
                    >
                      Book Now
                    </button>
                    <button
                      className="btn btn-light rounded-pill py-2 text-primary border-0"
                      onClick={() => navigate(`/cars/${car.id}`)}
                    >
                      View Details
                    </button>
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}

      <style>{`
        .fav-card { transition: transform 0.3s ease, box-shadow 0.3s ease; }
        .fav-card:hover { transform: translateY(-6px); box-shadow: 0 1rem 2rem rgba(0,0,0,0.15) !important; }
      `}</style>
    </div>
  );
}
