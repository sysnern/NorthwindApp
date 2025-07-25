import React from 'react';

interface Props {
  show: boolean;
  onConfirm: () => void;
  onClose: () => void;
}

const ConfirmModal: React.FC<Props> = ({ show, onConfirm, onClose }) => {
  if (!show) return null;
  return (
    <div className="modal show" style={{ display: 'block' }}>
      <div className="modal-dialog">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">Confirm</h5>
            <button type="button" className="btn-close" onClick={onClose}></button>
          </div>
          <div className="modal-body">Are you sure?</div>
          <div className="modal-footer">
            <button className="btn btn-secondary" onClick={onClose}>Cancel</button>
            <button className="btn btn-danger" onClick={onConfirm}>Delete</button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ConfirmModal;
