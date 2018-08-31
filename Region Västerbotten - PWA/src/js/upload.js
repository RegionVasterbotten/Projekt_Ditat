import axios from 'axios';
import Api from '../api'

export default function (url, file, name = 'image') {
  if (typeof url !== 'string') {
    throw new TypeError(`Expected a string, got ${typeof url}`);
  }
  const config = {
    headers: {
      'content-type': 'multipart/form-data'
    }
  };
  // You can add checks to ensure the url is valid, if you wish
  new ImageCompressor(file, {
    quality: .6,
    success(result) {
      const formData = new FormData();
      formData.append(name, file);
      console.log("Compressed!");

      // Send the compressed image file to server with XMLHttpRequest.
      Api()
        .post('', formData)
        .then(response => {
          console.log(response + "Upload success");
        })
        .catch(err => {
          console.log(err);
        });
    }
  })

  return axios.post(url, formData, config);
};
